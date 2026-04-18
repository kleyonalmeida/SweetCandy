# AGENTS.md — Application/Features/Budgets/Validations/

Validators da feature Budgets (FluentValidation).

Arquivos:
- ConvertBudgetToOrderCommandValidator.cs
- CreateBudgetCommandValidator.cs
- DeleteBudgetCommandValidator.cs
- UpdateBudgetCommandValidator.cs

Boas práticas:
- Validar enum com `.IsInEnum()` se aplicável.
- Mensagens claras para cada regra.
- Usar `IValidateMe` na command quando desejar validação automática.
