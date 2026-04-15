# Financas.Application

Camada de aplicação contendo serviços, DTOs e orquestração das regras de negócio.

## Estrutura

### Services
Serviços de aplicação para cada agregado:
- **EffectiveDateServices** - Operações CRUD para competências
- **ReceivableServices** - Gestão de contas a receber
- **PayableServices** - Gestão de contas a pagar

### Models
DTOs organizados por contexto:

#### EffectiveDateModels
- `EffectiveDateRequest` - Dados para criação/atualização
- `EffectiveDateSummary` - Resumo para listagem
- `EffectiveDateView` - Detalhes completos

#### ReceivableModels
- DTOs para operações com contas a receber

#### PayableModels
- DTOs para operações com contas a pagar

### Configuration
- **ServiceBase** - Classes base para serviços
- **ResultPattern** - Implementação do padrão Result para tratamento de erros
- **DependencyInjection** - Configuração de injeção de dependência

## Padrões Utilizados

- **Service Layer** - Encapsulamento da lógica de aplicação
- **DTO Pattern** - Transferência de dados entre camadas
- **Result Pattern** - Tratamento consistente de erros
- **Dependency Injection** - Inversão de controle

## Responsabilidades

- Orquestração de operações de negócio
- Validação de entrada
- Transformação entre entidades e DTOs
- Tratamento de erros e exceções
- Coordenação entre repositórios