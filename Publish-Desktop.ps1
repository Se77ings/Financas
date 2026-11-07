# Gera o instalador do programa fechado em artifacts\desktop\releases. Não publica nada sem -Upload.
# Uso, na raiz do repositório:
#   powershell -ExecutionPolicy Bypass -File .\Publish-Desktop.ps1 -Version 1.0.0
#   powershell -ExecutionPolicy Bypass -File .\Publish-Desktop.ps1 -Version 1.0.0 -Upload

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [switch]$Upload
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$repoUrl = 'https://github.com/Se77ings/Financas'
$project = "$root\src\Financas.Desktop\Financas.Desktop.csproj"
$webSource = "$root\src\Financas.Web"
$artifacts = "$root\artifacts\desktop"
$webCopy = "$artifacts\web"
$nodeModulesLink = "$webCopy\node_modules"
$publishDir = "$artifacts\publish"
$releasesDir = "$artifacts\releases"

function Step($message) { Write-Host "`n==> $message" -ForegroundColor Cyan }

# Comandos externos não disparam o catch sozinhos: o erro só aparece no código de saída.
function Invoke-Checked([string]$description, [scriptblock]$command) {
    & $command
    if ($LASTEXITCODE -ne 0) { throw "$description falhou (código $LASTEXITCODE)." }
}

# O rmdir sem /s só remove o vínculo e recusa uma pasta com conteúdo. Uma limpeza recursiva atravessaria
# a junction e apagaria o node_modules real.
function Remove-NodeModulesLink {
    if (Get-Item $nodeModulesLink -Force -ErrorAction SilentlyContinue) {
        Invoke-Checked 'Remoção do vínculo do node_modules' { cmd /c rmdir "$nodeModulesLink" }
    }
}

Set-Location $root

Step 'Restaurando o vpk'
Invoke-Checked 'dotnet tool restore' { dotnet tool restore }

Step 'Limpando a saída anterior'
Remove-NodeModulesLink
if (Test-Path $artifacts) { Remove-Item $artifacts -Recurse -Force }

# O .next de src\Financas.Web é o que o PM2 serve em produção, e o Next escreve nele mesmo com outro distDir.
# Por isso o build do programa acontece numa cópia.
Step 'Copiando o front'
robocopy $webSource $webCopy /E /XJ /NFL /NDL /NJH /NJS /NP /XD node_modules .next "$webSource\out" /XF .env*
if ($LASTEXITCODE -ge 8) { throw "robocopy falhou (código $LASTEXITCODE)." }

Invoke-Checked 'Vínculo do node_modules' { cmd /c mklink /J "$nodeModulesLink" "$webSource\node_modules" }

try {
    Step 'Gerando o front estático'
    Push-Location $webCopy
    $env:NEXT_PUBLIC_APP_MODE = 'desktop'
    Invoke-Checked 'npm run build:desktop' { npm run build:desktop }
}
finally {
    Remove-Item Env:\NEXT_PUBLIC_APP_MODE -ErrorAction SilentlyContinue
    Pop-Location
    Remove-NodeModulesLink
}

# public\components guarda os .tsx do front, e o export copia public\ inteira para out\.
Step 'Removendo código-fonte da saída do front'
$webOut = "$webCopy\out"
if (-not (Test-Path "$webOut\index.html")) { throw "O build do front não gerou $webOut\index.html." }
if (Test-Path "$webOut\components") { Remove-Item "$webOut\components" -Recurse -Force }

$sourceFiles = Get-ChildItem $webOut -Recurse -File -Include *.ts, *.tsx
if ($sourceFiles) { throw "Código-fonte na saída do front: $($sourceFiles.FullName -join ', ')" }

Step 'Publicando o executável'
Invoke-Checked 'dotnet publish' { dotnet publish $project -c Release -r win-x64 --self-contained true "-p:Version=$Version" -o $publishDir }

# O instalador é público. Os binários ficam de fora da busca por ConnectionStrings porque o nome da seção existe
# neles (Financas.Desktop e Microsoft.Extensions.Configuration); neles se procura uma conexão de verdade.
Step 'Conferindo que nenhuma configuração sensível entrou na saída'
$settingsFiles = Get-ChildItem $publishDir -Recurse -File -Filter 'appsettings*.json'
if ($settingsFiles) { throw "Arquivo de configuração na saída: $($settingsFiles.FullName -join ', ')" }

$textFiles = Get-ChildItem $publishDir -Recurse -File | Where-Object { $_.Extension -notin '.dll', '.exe', '.pdb' }
$leaks = $textFiles | Select-String -SimpleMatch 'ConnectionStrings' -List
if ($leaks) { throw "ConnectionStrings na saída: $($leaks.Path -join ', ')" }

$ownAssemblies = Get-ChildItem $publishDir -File -Include 'Financas.*.dll', 'Financeiro.*.dll', 'WageHistory.*.dll' -Recurse
foreach ($assembly in $ownAssemblies) {
    $text = [IO.File]::ReadAllText($assembly.FullName, [Text.Encoding]::Unicode)
    if ($text -match 'Trusted_Connection|Password=|User Id=') { throw "Conexão embutida em $($assembly.Name)." }
}

# A versão anterior é a base da atualização delta.
Step 'Baixando a versão publicada'
Invoke-Checked 'vpk download' { dotnet vpk download github --repoUrl $repoUrl --outputDir $releasesDir }

Step "Empacotando a versão $Version"
Invoke-Checked 'vpk pack' {
    dotnet vpk pack --packId Financeiro --packVersion $Version --packDir $publishDir --mainExe Financas.Desktop.exe `
        --packTitle Financeiro --packAuthors 'Gabriel Fernandes' --icon "$webSource\app\favicon.ico" `
        --runtime win-x64 --framework webview2 --outputDir $releasesDir
}

if ($Upload) {
    Step "Publicando a versão $Version no GitHub"
    $token = gh auth token
    if ($LASTEXITCODE -ne 0) { throw 'gh auth token falhou. Rode gh auth login.' }
    Invoke-Checked 'vpk upload' { dotnet vpk upload github --repoUrl $repoUrl --outputDir $releasesDir --publish --token $token }
    Remove-Variable token
}

Write-Host "`nConcluído. Instalador em $releasesDir." -ForegroundColor Green
