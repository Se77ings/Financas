# 🚀 Guia Prático de Atualização (Deploy Local)

Este guia contém os comandos exatos para atualizar o Backend (.NET) e o Frontend (Next.js) após alterações no código.

---

## ⚙️ Backend (.NET - Windows Service)

Rode os comandos no PowerShell como **Administrador** na raiz do projeto backend:

### 1. Parar o Serviço do Windows
sc.exe stop APIFinanceiro

### 2. Gerar o Build de Produção
dotnet publish .\src\Financas.API\Financas.API.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o C:\APIFinanceiro

### 3. Iniciar o Serviço do Windows
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