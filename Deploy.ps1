# Atualiza a produção local: backup, migrations, API (serviço do Windows) e front (PM2).
# Uso, no PowerShell como Administrador, na raiz do repositório:
#   powershell -ExecutionPolicy Bypass -File .\Deploy.ps1
#   powershell -ExecutionPolicy Bypass -File .\Deploy.ps1 -SkipFrontend

param(
    [string]$SqlServer = 'Gabriel',
    [string]$Database = 'Financas',
    [string]$ServiceName = 'APIFinanceiro',
    [string]$PublishDir = 'C:\APIFinanceiro',
    [string]$Pm2App = 'frontend-financeiro',
    [switch]$SkipFrontend
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$env:PATH += ";$env:USERPROFILE\.dotnet\tools"

function Step($message) { Write-Host "`n==> $message" -ForegroundColor Cyan }

# Comandos externos não disparam o catch sozinhos: o erro só aparece no código de saída.
function Invoke-Checked([string]$description, [scriptblock]$command) {
    & $command
    if ($LASTEXITCODE -ne 0) { throw "$description falhou (código $LASTEXITCODE)." }
}

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) { throw 'Rode o PowerShell como Administrador (necessário para parar e iniciar o serviço).' }

$branch = git -C $root branch --show-current
if ($branch -ne 'main') {
    $answer = Read-Host "A branch atual é '$branch', não 'main'. Publicar mesmo assim? (s/N)"
    if ($answer -ne 's') { return }
}

$migrationStarted = $false
$backupFile = "${Database}_$(Get-Date -Format 'yyyy-MM-dd_HHmm').bak"

try {
    Step "Parando o serviço $ServiceName"
    if ((Get-Service $ServiceName).Status -ne 'Stopped') { Stop-Service $ServiceName }

    Step "Backup de $Database ($backupFile, na pasta padrão de backup do SQL Server)"
    Invoke-Checked 'Backup' { sqlcmd -S $SqlServer -E -C -b -Q "BACKUP DATABASE [$Database] TO DISK = '$backupFile' WITH COPY_ONLY, INIT" }

    # Com a conta do Windows: o usuário da API só lê e grava dados. O --connection evita que o dotnet ef
    # use o appsettings.Development.json, que aponta para o FinancasHML.
    Step 'Aplicando as migrations'
    $migrationStarted = $true
    $connection = "Server=$SqlServer;Database=$Database;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
    Invoke-Checked 'Migrations' { dotnet ef database update --project "$root\src\Financeiro.Infra" --startup-project "$root\src\Financas.API" --configuration Release --connection $connection }

    Step "Publicando a API em $PublishDir"
    Invoke-Checked 'Publish' { dotnet publish "$root\src\Financas.API\Financas.API.csproj" -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o $PublishDir }

    Step "Iniciando o serviço $ServiceName"
    Start-Service $ServiceName
}
catch {
    Write-Host "`nFALHOU: $($_.Exception.Message)" -ForegroundColor Red
    if ($migrationStarted) {
        Write-Host "O serviço ficou parado. Se o banco ficou inconsistente, restaure o backup '$backupFile'." -ForegroundColor Yellow
    }
    else {
        Write-Host 'Nada foi alterado no banco; religando a API anterior.' -ForegroundColor Yellow
        Start-Service $ServiceName
    }
    exit 1
}

if ($SkipFrontend) { Write-Host "`nAPI atualizada (front ignorado)." -ForegroundColor Green; return }

try {
    Push-Location "$root\src\Financas.Web"

    Step 'Instalando dependências do front'
    Invoke-Checked 'npm install' { npm install --no-audit --no-fund }

    Step 'Gerando o build do front'
    Invoke-Checked 'npm run build' { npm run build }

    Step 'Reiniciando o front no PM2'
    pm2 describe $Pm2App *> $null
    if ($LASTEXITCODE -eq 0) { Invoke-Checked 'pm2 restart' { pm2 restart ecosystem.config.js --update-env } }
    else { Invoke-Checked 'pm2 start' { pm2 start ecosystem.config.js } }
    Invoke-Checked 'pm2 save' { pm2 save }
}
catch {
    Write-Host "`nFALHOU no front: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host 'A API já está atualizada. Corrija o erro e rode só os comandos do front (src\Financas.Web\Deploy.txt).' -ForegroundColor Yellow
    exit 1
}
finally {
    Pop-Location
}

Write-Host "`nDeploy concluído." -ForegroundColor Green
