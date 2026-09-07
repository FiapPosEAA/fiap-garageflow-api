# RFC 001: Plataforma e infraestrutura da solução

## Status
Accepted

## Contexto
A oficina está expandindo suas operações e exige segurança, alta disponibilidade e deploy automatizado.

## Decisão
Utilizar arquitetura em nuvem com Kubernetes, Terraform, API Gateway e autenticação baseada em CPF com JWT.

## Consequências
### Positivas
- escalabilidade horizontal
- desacoplamento de infraestrutura
- deploy automatizado
- maior segurança e padronização

### Negativas
- maior complexidade de operação
- necessidade de governança e manutenção de pipelines

## Justificativa
A combinação de Kubernetes + Terraform oferece melhor padronização, governança e reaproveitamento para ambiente corporativo.
