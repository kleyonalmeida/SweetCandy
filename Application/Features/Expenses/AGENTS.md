# AGENTS.md — Application/Features/Expenses/

Feature `Expenses` — gestão de despesas.

Arquivos na raiz:
- IExpenseService.cs

Subpastas: Commands/, DTOs/, Queries/, Validations/

Comportamento:
- `CreateExpenseCommand` valida `CategoryId` quando fornecido.
- Queries suportam filtros por data (`from`, `to`).
