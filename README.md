# Financeiro

Controle financeiro pessoal organizado por **competência** (mês/ano): recebimentos, gastos à vista, faturas de cartão com itens parcelados, assinaturas e recorrências, categorias e um dashboard.

## Arquitetura

| Projeto | Pasta | Papel |
|---|---|---|
| `Financeiro.Business` | `src/Financas.Business` | Domínio: agregados, validações (FluentValidation) e interfaces de repositório |
| `Financas.Application` | `src/Financas.Application` | Services, models (requests/views), Result pattern e registro de DI |
| `Financeiro.Infra` | `src/Financeiro.Infra` | EF Core 9 (SQL Server e SQLite): DbContext, mappings, migrations e repositórios |
| `Financeiro.Infra.Sqlite` | `src/Financeiro.Infra.Sqlite` | Migrations do SQLite, usadas pelo programa fechado |
| `Financas.API.Core` | `src/Financas.API.Core` | Controllers e registro da API, compartilhados pelos hosts |
| `Financas.API` | `src/Financas.API` | Host ASP.NET Core (.NET 9) do modo servidor, executado como serviço do Windows |
| `Financas.Web` | `src/Financas.Web` | Front em Next.js 16 / React 19, executado com PM2 |
| `Financas.Tests` | `src/Financas.Tests` | Testes xUnit: domínio e integração com SQLite em memória |
| `WageHistory.*` | `src/WageHistory` | API e banco separados do histórico de salários |
| `Financas.Presentation` | `src/Financas.Presentation` | Cliente .NET MAUI legado |

## Conceitos do domínio

- **Competência (`EffectiveDate`)**: mês/ano a que os lançamentos pertencem. Ao ser criada, gera as faturas dos cartões ativos, as parcelas que continuam do mês anterior e as recorrências vigentes.
- **Recebimento (`Receivable`)** e **gasto à vista (`ImmediateExpense`)**.
- **Fatura de cartão (`CardExpense`)**: soma dos seus itens (`CardExpense.Expense`), que podem ser parcelados (parcela x/y).
- **Recorrência (`RecurrentPayment`)**: assinatura de cartão ou gasto à vista recorrente (aluguel, internet...), com frequência e vigência.
- **Categoria (`Category`)**: obrigatória nos lançamentos novos; lançamentos antigos sem categoria são resolvidos em *Gerenciar Categorias*.

## Como executar (desenvolvimento)

1. **Banco:** SQL Server com o banco `FinancasHML`. Aplique as migrations:
   ```
   dotnet ef database update --project src/Financeiro.Infra --startup-project src/Financas.API --connection "<connection string do FinancasHML>"
   ```
2. **API:** `dotnet run --project src/Financas.API` (perfil de desenvolvimento em `http://localhost:5125`, Swagger habilitado).
3. **Front:** em `src/Financas.Web`, `npm install` e `npm run dev`. `NEXT_PUBLIC_API_URL` aponta para a API.
4. **Testes:** `dotnet test src/Financas.Tests`.

## Deploy (local)

A API roda como serviço do Windows e o front sob PM2 na mesma máquina. Veja `Update-Deploy.MD`. Em produção o Swagger fica desabilitado e as origens do CORS vêm de `AllowedOrigins`.
