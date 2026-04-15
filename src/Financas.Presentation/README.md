# Financas.Presentation

Interface do usuário desenvolvida em .NET MAUI seguindo o padrão MVVM.

## Estrutura

### Modules
Organização por funcionalidade:

#### EffectiveDate
- **Views**
  - `EffectiveDatePage` - Lista de competências
  - `EffectiveDateDetailsPage` - Detalhes da competência
- **ViewModels**
  - `EffectiveDateViewModel` - Lista e navegação
  - `EffectiveDateDetailsViewModel` - Detalhes e edição

#### Receivable (Contas a Receber)
- **Views**
  - `ReceivablePage` - Lista de contas a receber
  - `ReceivableFormPage` - Formulário para adicionar/editar
- **ViewModels**
  - `ReceivableViewModel` - Lista, navegação e exclusão
  - `ReceivableFormViewModel` - Formulário com validações

#### Payable (Contas a Pagar)
- **Views**
  - `ImmediateExpensePage` - Lista de despesas à vista
  - `ImmediateExpenseFormPage` - Formulário de despesas
- **ViewModels**
  - `ImmediateExpenseViewModel` - Lista e operações
  - `ImmediateExpenseFormViewModel` - Formulário com validações

### Configuration
- **DependencyInjection** - Registro de serviços do MAUI

## Padrões MAUI

- **MVVM** - Model-View-ViewModel
- **Data Binding** - Vinculação de dados
- **Commands** - Comandos para ações do usuário
- **ObservableCollection** - Coleções reativas
- **Shell Navigation** - Navegação entre páginas
- **TabBar** - Navegação por abas
- **SwipeView** - Ações de deslizar para excluir

## Tecnologias

- .NET MAUI
- XAML
- CommunityToolkit.Mvvm
- Shell Navigation
- Data Binding

## Funcionalidades

### Competências
- Lista de competências financeiras
- Navegação para detalhes
- Criação de novas competências

### Contas a Receber
- Lista com valores e datas
- Adicionar/Editar/Excluir
- Swipe para excluir
- Formulário com validações

### Despesas à Vista
- Lista de despesas
- Adicionar/Editar/Excluir
- Swipe para excluir
- Formulário completo com observações

## Navegação

O app utiliza TabBar com 3 abas principais:
1. **Competências** - Gerência de períodos
2. **A Receber** - Contas a receber
3. **Despesas** - Despesas à vista

## Padrão de Métodos

Todos os ViewModels seguem o padrão:
- `GetDataAsync()` - Carrega dados ao aparecer a tela
- `NavigateToAddCommand` - Navega para formulário de adição
- `NavigateToDetailsCommand` - Navega para edição
- `DeleteCommand` - Exclui item com confirmação
- `SaveCommand` - Salva dados do formulário