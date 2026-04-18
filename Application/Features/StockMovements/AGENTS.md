# AGENTS.md — Application/Features/StockMovements/

Feature `StockMovements` — controle de entradas/saídas de insumos.

Arquivos na raiz:
- IStockMovementService.cs

Subpastas: Commands/, DTOs/, Queries/, Validations/

Comportamento:
- `CreateStockMovementCommand` valida estoque disponível em saídas e registra movimento.
- Queries permitem filtrar por supplyId e orderId.
