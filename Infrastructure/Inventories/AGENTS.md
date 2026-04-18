# AGENTS.md — Infrastructure/Inventories/

Implementação do `IInventoryService` e serviços relacionados a inventário.

Arquivos:
- InventoryService.cs

Responsabilidade:
- CRUD de Supplies e FinalProducts.
- `GetInventoryAsync()` auto-cria inventário único se não existir.
- Operações de stock devem ser atômicas e gerar `StockMovement` quando apropriado.
