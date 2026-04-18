# AGENTS.md — Tests/Application.Tests/

Teste unitários da camada Application.

Arquivos de teste existentes:
- Features/Dashboard/GetDashboardQueryHandlerTests.cs
- Features/Expenses/CreateExpenseCommandHandlerTests.cs
- Features/MonthlyGoals/UpsertMonthlyGoalCommandHandlerTests.cs
- Features/Orders/UpdateOrderCommandHandlerTests.cs
- Features/StockMovements/CreateStockMovementCommandHandlerTests.cs

Padrões:
- Usar NSubstitute para mockar dependências
- Nomenclatura: MetodoTestado_Cenario_ResultadoEsperado
