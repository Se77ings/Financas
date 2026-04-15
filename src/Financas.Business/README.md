# Financeiro.Business

Camada de domínio do sistema financeiro, contendo as entidades principais e regras de negócio.

## Estrutura

### Aggregates
- **EffectiveDate** - Competência financeira (mês/ano) que agrupa receitas e despesas
- **Receivable** - Contas a receber
- **Payable** - Contas a pagar (classe base)
  - **ImmediateExpense** - Despesas à vista
  - **CardExpense** - Despesas no cartão de crédito

### Interfaces
Contratos para repositórios de cada agregado:
- `IEffectiveDateRepository`
- `IReceivableRepository` 
- `IPayableRepository`
- `ICardExpenseRepository`

### Validations
Validadores para cada entidade usando FluentValidation:
- `EffectiveDateValidator`
- `ReceivableValidator`
- `CardExpenseValidator`
- `ImmediateExpenseValidator`

## Regras de Negócio

- Uma competência pode ter múltiplas receitas e despesas
- O cálculo do saldo é: Total Receitas - Total Despesas
- Cada entidade possui validações específicas
- Todas as entidades herdam de `Entity` com Id e CreatedAt

## Linguagem Ubíqua

- **Subtotal** - Valor sem descontos
- **Total** - Valor com descontos aplicados
- **Competência** - Período de referência (mês/ano)
- **Direito** - Valor a receber
- **Obrigação** - Valor a pagar