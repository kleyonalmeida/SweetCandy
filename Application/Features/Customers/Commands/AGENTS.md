# AGENTS.md — Application/Features/Customers/Commands/

Commands para Customers.

Arquivos:
- CreateCustomerCommand.cs
- DeleteCustomerCommand.cs
- UpdateCustomerCommand.cs

Padrão: requests simples, validators garantem integridade, handlers delegam ao `ICustomerService`.
