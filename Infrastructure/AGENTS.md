# AGENTS.md — Infrastructure/

Camada `Infrastructure` — implementações de serviços, persistence e migrations.

Arquivos: Infrastructure.csproj
Subpastas: Budgets/, Categories/, Customers/, Expenses/, Inventories/, Migrations/, MonthlyGoals/, Orders/, Persistence/, Receipts/, StockMovements/

Responsabilidade:
- Implementar interfaces definidas em `Application` (ex: `IBudgetService`).
- Gerenciar `AppDbContext` e migrations EF Core.

Boas práticas:
- Serviços registrados como Scoped no `Program.cs` (WebApi).
- Operações que alteram estoque devem ser atômicas (transação/SaveChanges segura).
