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