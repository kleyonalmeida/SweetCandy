# AGENTS.md — Infrastructure/Budgets/

Implementação do `IBudgetService`.

Arquivos:
- BudgetService.cs

Responsabilidade:
- CRUD de Budgets com `AppDbContext`.
- Incluir itens (`BudgetItem`) com cascade delete configurado.
- Tratar conversão de Budget para Order quando solicitado.
