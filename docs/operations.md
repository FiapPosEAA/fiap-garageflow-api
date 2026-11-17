# Operação e governança

## GitHub Environments

Criar os ambientes `homologacao` e `producao` nos quatro repositórios. O ambiente `producao` deve exigir aprovação antes de executar o CD.

Secrets comuns:

- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`
- `JWT_SECRET`

Secrets da API:

- `DEFAULT_CONNECTION`

Secrets da Lambda:

- `LAMBDA_SUBNET_IDS` em JSON, por exemplo `[...]`
- `LAMBDA_SECURITY_GROUP_IDS` em JSON, por exemplo `[...]`
- `DB_HOST`
- `DB_NAME`
- `DB_USER`
- `DB_PASSWORD`

Variables comuns:

- `AWS_REGION`, normalmente `us-east-2`

Variable da API:

- `EKS_CLUSTER_NAME`

## Proteção de branches

Aplicar em `main` e, quando criada, `homologacao`:

- exigir Pull Request antes do merge;
- exigir pelo menos uma aprovação;
- exigir os checks de CI do repositório;
- exigir branch atualizada antes do merge;
- bloquear force push e exclusão da branch;
- restringir bypass a administradores autorizados.

Essas regras são configurações do GitHub e não são inferíveis nem ativáveis apenas pelos workflows versionados.

## Evolução para microsserviços (Fase 4)

O conteúdo acima vale para o monólito e a Lambda. Com a migração, a governança cobre também os repositórios `garageflow-os-service`, `garageflow-billing-service`, `garageflow-execution-service` e `garageflow-ui`, além de `fiap-garageflow-db-infra` e `fiap-garageflow-k8s-infra` (a menção a "quatro repositórios" ficou desatualizada).

Os workflows de CD dos três microsserviços usam o Environment `producao` (não há `homologacao` neles). Nomes dos secrets usados (valores nunca devem ser documentados):

- OS: `OS_CONNECTION`, `OS_SQS_QUEUE_URL`, `JWT_SECRET`;
- Billing: `BILLING_CONNECTION`, `BILLING_SQS_QUEUE_URL`, `JWT_SECRET`;
- Execution: `EXECUTION_SQS_QUEUE_URL`, `JWT_SECRET`;
- Terraform de SQS e DynamoDB: `TF_STATE_BUCKET`, `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`;
- variables: `AWS_REGION` e `EKS_CLUSTER_NAME`.

As filas SQS e a tabela DynamoDB são provisionadas por workflows manuais (`Provision SQS` no OS Service e `Provision DynamoDB` no Execution Service). O `DEFAULT_CONNECTION` listado acima continua sendo usado apenas pelo monólito.