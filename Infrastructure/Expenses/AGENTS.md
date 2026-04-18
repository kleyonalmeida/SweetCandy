# AGENTS.md — Infrastructure/Expenses/

Implementação do `IExpenseService`.

Arquivos:
- ExpenseService.cs

Responsabilidade:
- CRUD de Expense e mapeamento de `PaymentMethod`.
- Filtro por período nas queries (from/to).
- FK opcional para `Category` (set null quando categoria deletada).
