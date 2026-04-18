# AGENTS.md — Application/Features/Categories/

Feature `Categories` — CRUD de categorias.

Arquivos na raiz da feature:
- ICategoryService.cs

Subpastas:
- Commands/: Create/Update/Delete
- DTOs/: CategoryResponse, CreateCategoryRequest, UpdateCategoryRequest
- Queries/: GetCategoriesQuery, GetCategoryByIdQuery
- Validations/: Validators correspondentes

Regras:
- `CreateCategoryCommand` deve validar nome único (via service se necessário).
