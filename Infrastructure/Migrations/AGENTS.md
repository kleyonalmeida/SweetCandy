# AGENTS.md — Infrastructure/Migrations/

Migrations EF Core geradas para o schema do banco.

Arquivos:
- 20260401184407_InitialCreate.cs
- 20260401184407_InitialCreate.Designer.cs
- 20260402142854_AddPaymentMethodToExpense.cs
- 20260402142854_AddPaymentMethodToExpense.Designer.cs
- 20260407175149_AddExpenseCategoryFK.cs
- 20260407175149_AddExpenseCategoryFK.Designer.cs
- AppDbContextModelSnapshot.cs

Como gerar nova migration:
```bash
dotnet ef migrations add NomeDaMigration --project Infrastructure --startup-project WebApi
```

Aplicar migrations em startup: `db.Database.Migrate()` (já configurado em `Program.cs`).
