# AGENTS.md — Application/Features/Orders/

Feature `Orders` — gestão de pedidos.

Arquivos na raiz:
- IOrdersService.cs

Subpastas: Commands/, DTOs/, Queries/, Validations/

Comportamento:
- `CreateOrderCommand` valida cliente e itens, calcula total.
- `UpdateOrderCommand` atualiza campos e itens.
