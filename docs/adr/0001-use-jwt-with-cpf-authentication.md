# ADR 0001: Uso de JWT com autenticação por CPF

## Status
Accepted

## Contexto
As APIs sensíveis precisam de proteção e o cliente requer autenticação com base em CPF.

## Decisão
A autenticação será feita por CPF validado na API, seguida de geração de JWT para acesso às rotas protegidas.

## Rationale
Esse modelo reduz a dependência de credenciais de usuário e se alinha ao requisito da solução: autenticação simples, segura e compatível com operações do cliente.

## Consequências
- rotas sensíveis protegidas
- padronização de tokens JWT
- integração mais simples com API Gateway e serviços futuros
