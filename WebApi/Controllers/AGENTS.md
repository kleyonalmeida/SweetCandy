# AGENTS.md — WebApi/Controllers/

Controllers HTTP da API. Usam `ApiControllerBase` para mapear `IResponseWrapper` para HTTP.

Arquivos:
- ApiControllerBase.cs
- BudgetsController.cs
- CategoriesController.cs
- CustomersController.cs
- DashboardController.cs
- ExpensesController.cs
- InventoriesController.cs
- MonthlyGoalsController.cs
- OrdersController.cs
- ReceiptsController.cs
- StockMovementsController.cs

Padrão de cada controller:
- Injeta `IMediator`
- Recebe HTTP request, cria Command/Query e `await mediator.Send(request)`
- Retorna `FromResponse(result)` (mapeia sucesso/falha para 200/400)

Rotas e endpoints estão documentados no AGENTS.md raiz.
