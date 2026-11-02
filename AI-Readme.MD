# AI-Readme

Referência rápida de convenções e decisões deste repositório, para sessões com assistentes. A visão geral do produto e da arquitetura está no `README.md`; as regras de commit e comentários, no `CLAUDE.md` da pasta `C:\Github`.

## Camadas

- **Business** (domínio) não depende de nada. **Infra** depende do Business.
- **Application** referencia **Business** e **Infra**: o registro de DI dos repositórios fica em `Financas.Application/Configuration/DependencyInjection.cs` (`RegisterServices`). É uma decisão do dono do projeto; não mover para um composition root separado.
- **API** só referencia a Application. Controllers herdam de `ApiControllerBase`.

## Result pattern e HTTP

- Services retornam `Result<T>` (consultas) ou `Result<bool>` (comandos). Não usar tuplas `(Result<bool>, View)`.
- Registro inexistente: `Error.NotFound("...")`. Regra de negócio ou validação: `new Error("...")`. Sem códigos em string.
- Nos controllers: `CommandResult(result)` → 200 sem corpo; `QueryResult(result)` → 200 com o valor; não encontrado → 404; demais falhas → 400 com a mensagem em texto puro.

## Validação

- Fluxo de edição: carregar a entidade → aplicar `Update(...)` → validar a própria entidade com `ValidateEntityAsync` → salvar só se válido. Não criar entidades "descartáveis" só para validar.
- As mesmas regras básicas também são validadas no front, antes de enviar.

## Persistência (EF Core)

- O `FinanceiroDBContext` usa `NoTracking` global e `AutoDetectChangesEnabled = false`. **Decisão: manter.** Toda gravação é explícita:
  - entidade nova: `Add`;
  - entidade carregada e alterada: `Update` (percorre o grafo) ou `Entry(x).State = Modified` (só a raiz);
  - alteração em lote: `ExecuteUpdateAsync`.
  - Não use `Update` num grafo que contém um filho **novo**: o Id é gerado no construtor e o EF o marcaria como `Modified` (UPDATE numa linha inexistente). Veja `ICardExpenseRepository.AddItem`.
- Todas as FKs ficam com `ClientSetNull` (forçado no `OnModelCreating`). **Decisão: manter.** Exclusões em cascata não existem no modelo; as exclusões são bloqueadas pelos services quando há dependentes (cartão com faturas, categoria em uso, competência com lançamentos). Exceção só no banco: `FK_Payable_EffectiveDate_EffectiveDateId` tem cascade, criado pela migration manual `CascadeDeleteOnFKs`.
- Operações que gravam em várias etapas usam `ITransactionRunner` (criação de competência, parcelamento com assinatura, transferência de itens).
- **Migrations:** sempre revise o SQL (`dotnet ef migrations script`). Renomear entidade gera `DROP TABLE` + `CREATE TABLE` (perda de dados); nesses casos a migration é reescrita à mão com `RenameTable`/`RenameIndex` (ex.: `RenameSubscriptionToRecurrentPayment`).

## Domínio

- A fatura (`CardExpense`) é a soma dos itens. Em somas de gastos, conte só `ImmediateExpense` e `CardExpense.Expense`, nunca a fatura junto com os itens.
- Itens entram na fatura por `CardExpense.AddExpense`, que recalcula os totais.
- `RecurrentPayment.IsDueIn(mês, ano)` decide se uma recorrência gera lançamento na competência.
- Datas: cada parcela tem a sua data (a parcela N = data da compra + N−1 meses).

## Front (Next.js)

- `src/Financas.Web/AGENTS.md`: esta versão do Next.js tem mudanças em relação ao conhecimento comum; consulte `node_modules/next/dist/docs/`.
- Services chamam a API direto (`NEXT_PUBLIC_API_URL`). Erros da API vêm como texto: use `readErrorAsync`.
- Padrões existentes: modais com fundo `#1a1917`, cabeçalho/rodapé `#151413`; categoria via `CategoryComboBox`; gráficos com `recharts`.

## Testes

- `src/Financas.Tests`: domínio (unitários) e integração com SQLite em memória (`TestDatabase`), passando pelos mesmos services da API. Cada teste começa com banco limpo.
- Ao mexer no gerador de competências, no parcelamento ou nos totais da fatura, rode `dotnet test`.
