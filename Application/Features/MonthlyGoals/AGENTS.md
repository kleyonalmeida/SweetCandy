# AGENTS.md — Application/Features/MonthlyGoals/

Feature `MonthlyGoals` — metas mensais financeiras.

Arquivos na raiz:
- IMonthlyGoalService.cs

Subpastas: Commands/, DTOs/, Queries/, Validations/

Comportamento:
- `UpsertMonthlyGoalCommand` suporta valor fixo (`TargetAmount`) ou `% sobre custos` (`PercentageOverCosts`).
- `GetMonthlyGoalByMonthQuery` retorna meta do mês.
