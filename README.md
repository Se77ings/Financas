# Sistema Financeiro

Sistema de controle financeiro desenvolvido em .NET com arquitetura em camadas, seguindo princípios de Domain Driven Design (DDD) e Clean Architecture.

## Arquitetura

A solução está organizada em 4 projetos principais:

- **Financeiro.Business** - Camada de domínio com entidades e regras de negócio
- **Financeiro.Infra** - Camada de infraestrutura com acesso a dados
- **Financas.Application** - Camada de aplicação com serviços e DTOs
- **Financas.Presentation** - Interface do usuário em .NET MAUI

## Conceitos do Domínio

- **Competência (EffectiveDate)** - Período de referência (mês/ano)
- **Contas a Receber (Receivable)** - Valores a receber
- **Contas a Pagar (Payable)** - Valores a pagar
- **Despesa à Vista (ImmediateExpense)** - Gastos imediatos
- **Despesa Cartão (CardExpense)** - Gastos no cartão de crédito

## Tecnologias

- .NET 8
- Entity Framework Core
- .NET MAUI
- SQLite/SQL Server
- MVVM Pattern

## Como Executar

1. Clone o repositório
2. Restaure os pacotes NuGet
3. Execute as migrations do Entity Framework
4. Defina o projeto Financas.Presentation como startup
5. Execute a aplicação