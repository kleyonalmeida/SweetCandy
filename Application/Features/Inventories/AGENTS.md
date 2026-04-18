# AGENTS.md — Application/Features/Inventories/

Feature `Inventories` — insumos, produtos finais, receita e movimentações.

Arquivos na raiz:
- IInventoryService.cs

Subpastas: Commands/, DTOs/, Queries/, Validations/

Comportamento-chave:
- `AdjustSupplyStockCommand` ajusta `Supply.Quantity` e cria `StockMovement` de forma atômica.
- `GetInventoryQuery` auto-cria o inventário único se ausente.
- `CreateFinalProductCommand` valida `CategoryId`.
