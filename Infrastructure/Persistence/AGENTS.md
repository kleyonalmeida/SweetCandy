# AGENTS.md — Infrastructure/Persistence/

Persistência: `AppDbContext` e configuração de EF Core.

Arquivos:
- AppDbContext.cs

Responsabilidade:
- Declarar DbSets: Budgets, BudgetItems, Orders, OrderItems, Customers, Categories, Expenses, FinalProducts, RecipeItems, Supplies, Inventories, StockMovements, Receipts, MonthlyGoals
- Configurar relacionamentos (cascade, set null)
- Incluir configuração de conversões/indices se necessário

Como adicionar nova entidade:
1. Criar entidade em `Domain/Entities`
2. Adicionar DbSet em `AppDbContext`
3. Criar migration e aplicar
