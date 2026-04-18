# AGENTS.md — Application/Features/Budgets/Commands/

Contém commands para a feature Budgets.

Arquivos:
- ConvertBudgetToOrderCommand.cs
- CreateBudgetCommand.cs
- DeleteBudgetCommand.cs
- UpdateBudgetCommand.cs

Padrão:
- Cada command é um `record` que implementa `IRequest<IResponseWrapper<T>>`.
- Validators correspondentes estão em Validations/.
- Handlers pertencem ao Application e usam services/ports para persistência.
