# Gera a migration nos dois bancos: SQL Server (modo servidor) e SQLite (programa fechado).
# Não aplica nada em banco nenhum. Uso, na raiz do repositório:
#   powershell -ExecutionPolicy Bypass -File .\Add-Migration.ps1 -Name NomeDaMigration

param(
    [Parameter(Mandatory = $true)]
    [string]$Name
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

Step "SQL Server: $Name"
Invoke-Checked 'Migration do SQL Server' { dotnet ef migrations add $Name --project "$root\src\Financeiro.Infra" --startup-project "$root\src\Financas.API" }

Step "SQLite: $Name"
Invoke-Checked 'Migration do SQLite' { dotnet ef migrations add $Name --project "$root\src\Financeiro.Infra.Sqlite" --startup-project "$root\src\Financeiro.Infra.Sqlite" --output-dir Migrations }

Write-Host "`nRevise as duas migrations. Uma recriação de tabela no SQLite perde o cascade de FK_Payable_EffectiveDate_EffectiveDateId, feito à mão na InitialCreate." -ForegroundColor Yellow
