# Backup completo do banco, verificado após a gravação.
# Uso, na raiz do repositório:
#   powershell -ExecutionPolicy Bypass -File .\Backup-Database.ps1
#   powershell -ExecutionPolicy Bypass -File .\Backup-Database.ps1 -Database FinancasHML -Label antes-ajuste

param(
    [string]$SqlServer = 'Gabriel',
    [string]$Database = 'Financas',
    [string]$Label = ''
)

$ErrorActionPreference = 'Stop'

$suffix = if ($Label) { "_$Label" } else { '' }
$fileName = "${Database}_$(Get-Date -Format 'yyyy-MM-dd_HHmmss')$suffix.bak"

# Sem caminho, o arquivo vai para a pasta padrão de backup do SQL Server: é a que a conta do serviço do SQL
# com certeza consegue gravar.
$backupDir = (sqlcmd -S $SqlServer -E -C -b -h -1 -W -Q "SET NOCOUNT ON; SELECT SERVERPROPERTY('InstanceDefaultBackupPath')" | Select-Object -First 1).Trim()
if ($LASTEXITCODE -ne 0 -or -not $backupDir) { throw "Não foi possível conectar em $SqlServer." }
$backupPath = Join-Path $backupDir $fileName

Write-Host "Fazendo backup de $Database em $backupPath ..." -ForegroundColor Cyan
# COPY_ONLY: não interfere numa eventual sequência de backups do próprio SQL Server.
sqlcmd -S $SqlServer -E -C -b -Q "BACKUP DATABASE [$Database] TO DISK = N'$backupPath' WITH COPY_ONLY, INIT, CHECKSUM, STATS = 25"
if ($LASTEXITCODE -ne 0) { throw 'O backup falhou.' }

Write-Host 'Verificando o arquivo ...' -ForegroundColor Cyan
sqlcmd -S $SqlServer -E -C -b -Q "RESTORE VERIFYONLY FROM DISK = N'$backupPath' WITH CHECKSUM"
if ($LASTEXITCODE -ne 0) { throw "O arquivo $backupPath foi gerado, mas não passou na verificação." }

Write-Host "`nBackup concluído e verificado: $backupPath" -ForegroundColor Green
