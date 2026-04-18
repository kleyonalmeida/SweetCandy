# AGENTS.md — Infrastructure/Categories/

Implementação do `ICategoryService`.

Arquivos:
- CategoryService.cs

Responsabilidade:
- CRUD de Category via `AppDbContext`.
- Garantir integridade referencial com `FinalProduct` (set null quando categoria removida).
