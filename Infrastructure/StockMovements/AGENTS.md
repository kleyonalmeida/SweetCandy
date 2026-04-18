# AGENTS.md — Infrastructure/StockMovements/

Implementação do `IStockMovementService`.

Arquivos:
- StockMovementService.cs

Responsabilidade:
- Criar StockMovement e ajustar `Supply.Quantity` atomicamente.
- Em movimentos do tipo `Saida`, validar estoque suficiente e retornar erro caso contrário.
- Consultas por `SupplyId` e `OrderId`.
