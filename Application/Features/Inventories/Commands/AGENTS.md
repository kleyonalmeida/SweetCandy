# AGENTS.md — Application/Features/Inventories/Commands/

Commands para Inventories.

Arquivos:
- AddRecipeItemCommand.cs
- AdjustSupplyStockCommand.cs
- CreateFinalProductCommand.cs
- CreateSupplyCommand.cs
- DeleteFinalProductCommand.cs
- DeleteSupplyCommand.cs
- RemoveRecipeItemCommand.cs
- UpdateFinalProductCommand.cs
- UpdateSupplyCommand.cs

Observações:
- Ajustes de estoque devem validar quantidade e criar StockMovement.
- Recipe items vinculam `FinalProduct` a `Supply`.
