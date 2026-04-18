# AGENTS.md — Application/Features/Customers/Validations/

Validators para Customers.

Arquivos:
- CreateCustomerCommandValidator.cs
- DeleteCustomerCommandValidator.cs
- UpdateCustomerCommandValidator.cs

Boas práticas: validar email com `.EmailAddress()` e campos obrigatórios com `.NotEmpty()`.
