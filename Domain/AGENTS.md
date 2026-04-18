# AGENTS.md — Domain/

Camada `Domain` — entidades e regras de negócio puras.

Arquivos: Domain.csproj
Subpastas relevantes: Entities/, Enums/

Responsabilidade:
- Definir modelos de domínio (`BaseEntity`, entidades de negócio).
- Não referenciar outras camadas (nenhuma dependência externa).
- Conter validações de invariantes simples (se aplicável) e lógica de entidade.

Boas práticas:
- IDs como GUID strings.
- Não injetar serviços ou EF Core aqui.
