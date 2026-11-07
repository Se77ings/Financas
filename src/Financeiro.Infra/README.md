# Financeiro.Infra

Camada de infraestrutura responsável pelo acesso a dados e persistência.

## Estrutura

### Context
- **FinanceiroDBContext** - Contexto do Entity Framework Core
- **DatabaseRegistration** - Registra o contexto conforme `Database:Provider`: `SqlServer` (padrão, modo servidor) ou `Sqlite` (programa fechado)

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

As migrations do SQLite ficam em `src/Financeiro.Infra.Sqlite`, com histórico próprio a partir de `InitialCreate`. Cada mudança no modelo precisa de uma migration nos dois projetos:

```powershell
powershell -ExecutionPolicy Bypass -File .\Add-Migration.ps1 -Name NomeDaMigration
```

Revise as duas antes de aplicar. O cascade de `FK_Payable_EffectiveDate_EffectiveDateId` existe só no banco: se uma migration do SQLite recriar a tabela `Payable`, ele precisa ser refeito à mão, e o `SqliteMigrationTests` falha caso isso seja esquecido.

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