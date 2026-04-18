# AGENTS.md — Application/Features/Expenses/Validations/

Validators para Expenses.

Arquivos:
- CreateExpenseCommandValidator.cs
- DeleteExpenseCommandValidator.cs
- UpdateExpenseCommandValidator.cs

Boas práticas: validar `Value > 0`, `Date` coerente e `CategoryId` existente via serviço.
