# Documentação de Arquitetura

## Visão geral

A aplicação GarageFlow Service foi projetada para suportar gestão de oficina mecânica em ambiente corporativo, priorizando segurança, organização em camadas, escalabilidade e infraestrutura automatizada.

## Objetivo

Disponibilizar uma API robusta para gestão de clientes, veículos, serviços, peças e ordens de serviço, mantendo autenticação segura com JWT e suporte a deploy em Kubernetes.

## Visão de componentes

```mermaid
flowchart LR
    Client[Cliente / Integrador] --> APIGW[API Gateway HTTP]
    APIGW --> AUTH[Lambda Auth]
    APIGW --> API[GarageFlowService.API]
    API --> APP[GarageFlowService.Application]
    APP --> DOMAIN[GarageFlowService.Domain]
    APP --> INFRA[GarageFlowService.Infrastructure]
    INFRA --> DB[(SQL Server / RDS)]
    AUTH --> DB
    API --> SWAGGER[Swagger OpenAPI]
    MON[Observabilidade - parceiro] --> API
```

## Fluxo principal

### Autenticação por CPF

```mermaid
sequenceDiagram
    participant C as Cliente
    participant G as API Gateway
    participant A as Lambda Auth
    participant R as Customer Repository
    participant D as Banco de Dados

    C->>G: POST /auth { cpf }
    G->>A: Invoca função serverless
    A->>A: Valida CPF
    A->>R: Busca cliente por documento
    R->>D: SELECT por Document
    D-->>R: Cliente encontrado
    R-->>A: Dados do cliente
    A->>A: Verifica status ativo
    A->>A: Gera JWT
    A-->>G: Token JWT
    G-->>C: Token + payload do cliente
```

### Abertura de ordem de serviço

```mermaid
sequenceDiagram
    participant C as Cliente / Operador
    participant G as API Gateway
    participant A as GarageFlowService.API
    participant U as CreateWorkOrderHandler
    participant R as Repositórios
    participant D as SQL Server RDS

    C->>G: POST /api/workorders com Bearer JWT
    G->>A: Encaminha requisição
    A->>A: Valida assinatura e expiração
    A->>U: Executa comando de abertura
    U->>R: Valida cliente e veículo
    R->>D: Persiste ordem de serviço
    D-->>R: Ordem criada
    R-->>U: Entidade persistida
    U-->>A: DTO da ordem
    A-->>C: 201 Created
```

## Decisões arquiteturais

### 1. Clean Architecture
A separação entre Domain, Application, Infrastructure e API mantém a lógica de negócio isolada da tecnologia.

### 2. JWT para autenticação
O acesso às rotas protegidas é autorizado por JWT validado pela API.

### 3. Persistência relacional
O banco relacional foi escolhido por oferecer consistência, consultas estruturadas e melhor aderência ao domínio da oficina.

### 4. Kubernetes e Terraform
A aplicação é implantada em ambiente Kubernetes, com infraestrutura provisionada por Terraform para automatização e reuso.

### 5. Escolha do banco de dados
O SQL Server gerenciado no Amazon RDS foi escolhido porque a aplicação utiliza .NET 8 e Entity Framework Core com o provider oficial `Microsoft.EntityFrameworkCore.SqlServer`. A escolha preserva transações ACID, constraints relacionais e consultas consistentes entre clientes, veículos, serviços, peças e ordens de serviço. O RDS privado reduz a superfície de exposição; somente workloads autorizados nas redes privadas devem acessar a porta 1433.

## Modelagem do banco

### Entidades principais
- Cliente
- Veículo
- Serviço
- Peça
- Ordem de Serviço
- Itens da Ordem de Serviço

### Relacionamentos principais
- Cliente possui vários veículos
- Ordem de Serviço pertence a um cliente e um veículo
- Ordem de Serviço contém vários serviços e peças

```mermaid
erDiagram
    CUSTOMER ||--o{ VEHICLE : possui
    CUSTOMER ||--o{ WORK_ORDER : solicita
    VEHICLE ||--o{ WORK_ORDER : relaciona
    WORK_ORDER ||--o{ WORK_ORDER_SERVICE : contem
    SERVICE ||--o{ WORK_ORDER_SERVICE : compoe
    WORK_ORDER ||--o{ WORK_ORDER_PART : utiliza
    PART ||--o{ WORK_ORDER_PART : compoe

    CUSTOMER {
        uuid id PK
        string name
        string document UK
        boolean is_active
    }
    VEHICLE {
        uuid id PK
        uuid customer_id FK
    }
    WORK_ORDER {
        uuid id PK
        uuid customer_id FK
        uuid vehicle_id FK
        string status
        decimal total_amount
    }
    SERVICE {
        uuid id PK
        decimal price
    }
    PART {
        uuid id PK
        decimal price
        integer stock
    }
    WORK_ORDER_SERVICE {
        uuid work_order_id FK
        uuid service_id FK
        decimal unit_price
    }
    WORK_ORDER_PART {
        uuid work_order_id FK
        uuid part_id FK
        decimal unit_price
        integer quantity
    }
```

`CUSTOMER.document` possui índice único para impedir duplicidade de CPF. `WORK_ORDER` referencia obrigatoriamente cliente e veículo. As tabelas de associação representam os relacionamentos muitos-para-muitos e armazenam o preço praticado no momento da ordem.

## Requisitos de segurança

- uso de JWT
- rotas protegidas por [Authorize]
- validação de CPF
- verificação de status do cliente
- uso de credenciais centralizadas em variáveis de ambiente

## Observações

A documentação acima atende ao escopo ajustado do projeto, sem incluir monitoramento e observabilidade, que ficaram a cargo do colega responsável.
