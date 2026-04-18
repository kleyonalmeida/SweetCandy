# AGENTS.md — Application/Features/

Contém todas as features organizadas por domínio (Budgets, Categories, Customers, Dashboard, Expenses, Inventories, MonthlyGoals, Orders, Receipts, StockMovements).

Responsabilidade:
- Cada pasta de feature agrupa Commands, Queries, DTOs e Validations.
- Interfaces de serviço (ex: `IBudgetService`) geralmente estão na raiz da pasta da feature.

Padrão de organização por feature:
- Commands/
- DTOs/
- Queries/
- Validations/

Criar novas features seguindo este padrão.
