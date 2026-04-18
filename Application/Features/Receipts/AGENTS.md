# AGENTS.md — Application/Features/Receipts/

Feature `Receipts` — recibos e pagamentos registrados.

Arquivos na raiz:
- IReceiptsService.cs

Subpastas: Commands/, DTOs/, Queries/, Validations/

Comportamento:
- `CreateReceiptCommand` valida existência do `OrderId` e `CustomerId` quando aplicável.
