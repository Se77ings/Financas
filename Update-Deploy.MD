# 🚀 Guia Prático de Atualização (Deploy Local)

Este guia contém os comandos exatos para atualizar o Backend (.NET) e o Frontend (Next.js) após alterações no código.

O script `Deploy.ps1`, na raiz, executa todos os passos abaixo em sequência (PowerShell como **Administrador**):

powershell -ExecutionPolicy Bypass -File .\Deploy.ps1

---

## ⚙️ Backend (.NET - Windows Service)

Rode os comandos no PowerShell como **Administrador** na raiz do projeto backend:

### 1. Parar o Serviço do Windows
sc.exe stop APIFinanceiro

### 2. Backup do banco
Vai para a pasta padrão de backup do SQL Server.

sqlcmd -S Gabriel -E -C -Q "BACKUP DATABASE Financas TO DISK = 'Financas_antes_deploy.bak' WITH COPY_ONLY, INIT"

### 3. Aplicar as migrations
Roda com a sua conta do Windows: o `usr_financas` da API só lê e grava dados, não altera tabelas. Sem o `--connection`, o `dotnet ef` usaria o `appsettings.Development.json` e aplicaria no `FinancasHML`.

dotnet ef database update --project .\src\Financeiro.Infra --startup-project .\src\Financas.API --configuration Release --connection "Server=Gabriel;Database=Financas;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"

Para criar uma migration nova, use o `Add-Migration.ps1`, que gera o mesmo nome no SQL Server e no SQLite (`src/Financeiro.Infra/README.md`).

powershell -ExecutionPolicy Bypass -File .\Add-Migration.ps1 -Name NomeDaMigration

### 4. Gerar o Build de Produção
dotnet publish .\src\Financas.API\Financas.API.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o C:\APIFinanceiro

### 5. Iniciar o Serviço do Windows
sc.exe start APIFinanceiro

---

## 💻 Frontend (Next.js - PM2)

Rode os comandos no PowerShell dentro da pasta `Financas.Web`:

### 1. Limpar processos ativos do PM2
pm2 delete all

### 2. Gerar o Build de Produção
npm run build

### 3. Subir a aplicação via PM2
pm2 start ecosystem.config.js

### 4. Salvar o estado no Windows
pm2 save

---

## 📦 Programa fechado (Windows)

Instalador para quem não tem SQL Server nem Node: API, front estático e SQLite num executável só. Não mexe na produção local (`Deploy.ps1`), que continua igual.

### 1. Gerar o instalador
Na raiz do repositório. O resultado fica em `artifacts\desktop\releases`, com `Financeiro-win-Setup.exe`, e nada é publicado.

powershell -ExecutionPolicy Bypass -File .\Publish-Desktop.ps1 -Version 1.0.0

O script faz o build do front numa cópia em `artifacts\desktop\web`, porque o `.next` de `src\Financas.Web` é o que o PM2 serve. Ele falha se encontrar `appsettings*.json`, `ConnectionStrings` ou código-fonte do front na saída.

### 2. Publicar a versão
Os programas instalados baixam a atualização do GitHub Releases na abertura seguinte. Use uma versão maior que a última publicada; o repositório é público, então o instalador também fica público.

powershell -ExecutionPolicy Bypass -File .\Publish-Desktop.ps1 -Version 1.0.1 -Upload

O `-Upload` usa o login do `gh` (`gh auth login`).

### Onde ficam os dados
Em `%LOCALAPPDATA%\Financeiro.Dados`, que a desinstalação não apaga: `financeiro.db`, `settings.json`, `logs\`, `webview\`, e as cópias automáticas `pre-migrate\` (antes de uma migration) e `pre-restore\` (antes de uma restauração).

### Migrations
Toda migration precisa existir nos dois bancos: use o `Add-Migration.ps1`. O programa aplica as do SQLite sozinho ao abrir e recusa restaurar um backup com migration que ele não conhece.
