# AGENTS.md — Domain/Entities/

Contém as entidades do domínio.

Arquivos presentes:
- BaseEntity.cs
- Budget.cs
- BudgetItem.cs
- Category.cs
- Customer.cs
- Expense.cs
- FinalProduct.cs
- Inventory.cs
- MonthlyGoal.cs
- Order.cs
- OrderItem.cs
- Receipt.cs
- RecipeItem.cs
- StockMovement.cs
- Supply.cs

Responsabilidade:
- Modelar propriedades e relacionamentos entre entidades.
- Implementar métodos de entidade (ex: AddItem, MarkUpdated, MarkAsPaid).

Regras:
- Evitar dependências externas; lógica de negócio somente.
- Usar strings GUID para `Id`.
