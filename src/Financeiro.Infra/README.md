# Financeiro.Infra

Camada de infraestrutura responsável pelo acesso a dados e persistência.

## Estrutura

### Context
- **FinanceiroDBContext** - Contexto do Entity Framework Core

### Mappings
Configurações de mapeamento objeto-relacional:
- `EffectiveDateMapping`
- `ReceivableMapping`
- `PayableMapping`
- `CardExpenseMapping`
- `ImmediateExpenseMapping`

### Repository
Implementações dos repositórios:
- `EffectiveDateRepository`
- `ReceivableRepository`
- `PayableRepository`
- `CardExpenseRepository`

### Migrations
Histórico de migrações do banco de dados:
- Initial - Criação inicial das tabelas
- UpdatingAggregates - Atualização dos agregados
- TPT_Correction - Correção da estratégia Table-Per-Type
- TranslatedColumnsToEnglish - Tradução das colunas para inglês

### ETL
Funcionalidades de importação e transformação de dados:
- **Importers** - Classes para importação de dados
- **Mappers** - Mapeadores de dados externos
- **Models** - Modelos para ETL

## Tecnologias

- Entity Framework Core
- SQL Server / SQLite
- Repository Pattern
- Table-Per-Type (TPT) para herança