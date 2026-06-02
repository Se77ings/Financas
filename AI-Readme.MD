# AI-Readme

Arquivo gerado automaticamente para registrar os padrões, convenções e decisões arquiteturais usadas neste repositório. Use este documento como referência rápida em sessões futuras com o assistente, para manter consistência e boas práticas.

---

## Visão geral do projeto

Este repositório contém uma aplicação multiplataforma com duas camadas principais:
- Backend: API ASP.NET Core (.NET 9)
- Frontend: .NET MAUI (cliente desktop/mobile) com padrão MVVM

Projetos principais (exemplos de pastas):
- `Financas.API` (Web API)
- `Financas.Application` (Serviços de aplicação)
- `Financas.Business` (Domínio / Aggregates / Entidades)
- `Financas.Infra` (Persistência / EF Core / Mappings)
- `Financas.Presentation` (MAUI UI, Pages + ViewModels)

---

## Padrões arquiteturais e camadas

- Arquitetura em camadas (separação de responsabilidades):
  - Presentation (MAUI) -> ViewModels -> Application services -> Domain -> Infra (Repository/EFCore) -> Database
- Domain-driven design (DDD) leve:
  - `Aggregates` e `Entities` no domínio (ex.: `EffectiveDate`, `Payable`, `CardExpense`).
  - Conversões implícitas utilizadas para mapear entidades -> view models (persistence -> DTOs) em alguns models.
- Repositório + Serviços de aplicação:
  - Repositories definem acesso ao banco (EF Core).
  - Services contêm regras de aplicação e validações envolvendo repositórios.
- Result pattern: `Result<T>` e `Error` para retornar sucesso/erro sem lançar exceções onde não necessário.

---

## Persistência / EF Core

- `FinanceiroDBContext` centraliza DbSets e configurações de mapeamento.
- Configurações comuns:
  - `OnModelCreating` aplica configurações via `ApplyConfigurationsFromAssembly`.
  - Strings `varchar(100)` para propriedades string por padrão.
- Evitar SQL dinâmico concatenado; use parâmetros ou `ExecuteSqlInterpolatedAsync` para evitar problemas de formatação e SQL injection.
- Para operações de atualização prefere atualizar a entidade e `SaveChangesAsync()` em vez de SQL bruto, quando possível.
- Use `EnableRetryOnFailure` em `UseSqlServer(...)` para resiliencia a falhas transitórias.

---

## Validação e regras

- FluentValidation é usado para validações de entidade (ex.: `EffectiveDateValidator`).
- Regras dependentes de persistência (ex.: "não criar competência duplicada") são checadas no serviço de aplicação usando o repositório (repository.Exists...).

---

## API (Web)

- Controllers usam `[ApiController]` e retornam `IActionResult`.
- Padrão de resposta: `Ok(...)` em sucesso; `BadRequest(result.Error.Message)` ou `NotFound(...)` em falha.
- Rotas exemplares: `api/EffectiveDate`, `api/Receivable`, `api/ImmediateExpense`, `api/CardExpense`.
- Endpoints que manipulam coleções aninhadas expõem rotas RESTful (ex.: `POST /api/CardExpense/{id}/items`).

---

## UI (.NET MAUI)

- Padrão MVVM com CommunityToolkit.Mvvm:
  - `[ObservableProperty]` para gerar propriedades, `[RelayCommand]` para comandos.
- Views: Pages XAML + code-behind minimal.
- ViewModels mantêm `ObservableCollection<T>` e expõem comandos e propriedades de binding.
- Uso de `CollectionView`, `BindableLayout`, `FlexLayout` e `Frame` para layout responsivo.
- Evite lógica complexa em code-behind; use bindings e comandos.

---

## Front-end React (Financas.Web)

- Localização: projeto cliente React (se presente no repositório como `Financas.Web`).
- Tipo: SPA React (recomendado usar Create React App / Vite). Preferir TypeScript para segurança de tipos.

- Comunicação com back-end:
  - Consome a API .NET (`Financas.API`) via chamadas HTTP REST JSON para rotas como `/api/Receivable`, `/api/CardExpense`, `/api/EffectiveDate`, etc.
  - Use um cliente HTTP centralizado (axios ou fetch wrapper) para configurar baseURL, timeouts e tratamento de erros consistente.
  - Em desenvolvimento, use proxy (ex.: `proxy` no package.json ou `vite` proxy) ou variáveis de ambiente (`REACT_APP_API_URL` / `VITE_API_URL`) para apontar para a API local.
  - Trate erros de rede e mensagens de validação retornadas pelo backend (status 4xx/5xx) e exiba mensagens amigáveis na UI.

- Padrão de rotas e arquitetura:
  - Use `react-router` para roteamento de páginas (ex.: `/effective-dates`, `/card-expenses/:id`).
  - Organização recomendada: `src/components` (UI atômica), `src/pages` (rotas/páginas), `src/services/api` (clientes HTTP), `src/hooks` (hooks reutilizáveis), `src/types` (tipos/DTOs), `src/store` (estado global — Context/Redux/Zustand).
  - Mantenha a lógica de chamadas à API em `services` e execute-as nos `hooks` ou nos `sagas/thunks` se usar Redux.

- State management e sincronização:
  - Para estados locais de componente use hooks (useState/useEffect). Para estados compartilhados entre páginas (lista de competências, auth) use Context ou Redux.
  - Considere SWR/React Query para cache e revalidação automática de dados (fetch, cache, refetch, retries).

- Autenticação / segurança (se aplicável):
  - Armazene tokens em memória ou cookie com flags HttpOnly (evitar localStorage se possível).
  - Para chamadas autenticadas, configure interceptors no cliente HTTP para anexar Authorization header.

- Build / Deploy:
  - Produza assets estáticos (`npm run build` / `yarn build`) e sirva via servidor (NGINX/IIS) ou incorpore no backend se desejar.
  - Configure variáveis de ambiente para apontar ao endpoint da API em produção.

- Boas práticas específicas:
  - Centralize mensagens de erro/labels para facilitar tradução (i18n) e consistência (pt-BR usado no app MAUI).
  - Testes: escreva testes unitários para componentes e testes de integração para fluxos críticos (Jest + React Testing Library).

---

## Convenções de código e nomenclatura

- Nomes em inglês para classes e arquivos (ex.: `CardExpense`, `EffectiveDate`) mas rótulos da UI em PT-BR.
- DTOs e Requests: `*Request`, `*View`, `*Summary`.
- ViewModels terminam com `ViewModel`, pages com `Page`.
- Use `async/await` em chamadas IO (services, repositories).

---

## Boas práticas e notas importantes

- Nunca concatenar valores em SQL diretamente; sempre usar parâmetros.
- Evitar `ChangeTracker.AutoDetectChangesEnabled = false` salvo quando necessário por performance e consciente das implicações.
- Trata erros em camada de serviço e retorne mensagens amigáveis via `Result<T>`.
- Ao mudar contratos (interfaces) reinicie o build completo; hot-reload não aplica alterações de assinatura.

---

## Como estender o projeto (passo a passo resumido)

1. Adicionar uma nova entidade/aggregate (Business): criar entidade e regras de domínio.
2. Criar mapeamento/Configuration em Infra (EntityTypeConfiguration) e aplicar via `OnModelCreating`.
3. Criar repository (Infra) e registrar no DI (`RegisterServices`).
4. Criar service de aplicação (Application) com validações e usar `Result<T>`.
5. Criar controller (API) com rotas RESTful para expor endpoints.
6. Criar DTOs `Request/View/Summary` para transporte entre camadas.
7. Criar View + ViewModel (Presentation) usando MVVM e CommunityToolkit para bindings.

---

## Padrões de segurança e produção

- Remover `EnableSensitiveDataLogging` em Production.
- Não comitar secrets; use `User Secrets` / Azure Key Vault / environment variables.
- Assinar pacotes MSIX para deploy Windows; configurar CI/CD para builds de release.

---

## Documentação e APIs

- Swagger está habilitado em `Financas.API` para desenvolvimento; utilize-o para inspecionar rotas e modelos.
- Para gerar documentação adicional, exporte o OpenAPI (Swagger) e complemente com exemplos das requests/responses.

---

Se quiser, eu posso:
- Gerar automaticamente um `API-summary.md` com a lista de endpoints e shapes (baseado nos models `*Request` e `*View`).
- Produzir um checklist de preparação para release (build/publish MSIX ou self-contained).

---

Fim do AI-Readme. Manterei este arquivo atualizado se você quiser adicionar convenções adicionais (ex.: estilo de commits, regras de PR, CI).