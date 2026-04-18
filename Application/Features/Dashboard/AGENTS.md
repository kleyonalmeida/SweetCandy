# AGENTS.md — Application/Features/Dashboard/

Feature `Dashboard` — agregação financeira e métricas.

Arquivos:
- DTOs/: DashboardResponse.cs
- Queries/: GetDashboardQuery.cs

Comportamento:
- `GetDashboardQuery` calcula receitas, despesas, lucro e progresso da meta mensal.
- Não possui Validations/ (query simples).
