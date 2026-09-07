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