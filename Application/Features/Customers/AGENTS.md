# AGENTS.md — Application/Features/Customers/

Feature `Customers` — CRUD de clientes.

Arquivos na raiz:
- ICustomerService.cs

Subpastas: Commands/, DTOs/, Queries/, Validations/

Comportamento:
- `CreateCustomerCommand` valida e persiste cliente via `ICustomerService`.
- Emails e telefones têm formatos validados nos validators.
