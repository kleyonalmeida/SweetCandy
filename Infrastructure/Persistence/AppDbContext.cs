using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Budget> Budgets => Set<Budget>();
  public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
  public DbSet<Order> Orders => Set<Order>();
  public DbSet<OrderItem> OrderItems => Set<OrderItem>();
  public DbSet<Customer> Customers => Set<Customer>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Expense> Expenses => Set<Expense>();
  public DbSet<FinalProduct> FinalProducts => Set<FinalProduct>();
  public DbSet<RecipeItem> RecipeItems => Set<RecipeItem>();
  public DbSet<Supply> Supplies => Set<Supply>();
  public DbSet<Inventory> Inventories => Set<Inventory>();
  public DbSet<StockMovement> StockMovements => Set<StockMovement>();
  public DbSet<Receipt> Receipts => Set<Receipt>();
  public DbSet<MonthlyGoal> MonthlyGoals => Set<MonthlyGoal>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Budget → BudgetItem (one-to-many, cascade)
    modelBuilder.Entity<Budget>()
      .HasMany(b => b.Items)
      .WithOne(i => i.Budget)
      .HasForeignKey(i => i.BudgetId)
      .OnDelete(DeleteBehavior.Cascade);

    // Order → OrderItem (one-to-many, cascade)
    modelBuilder.Entity<Order>()
      .HasMany(o => o.Items)
      .WithOne(i => i.Order)
      .HasForeignKey(i => i.OrderId)
      .OnDelete(DeleteBehavior.Cascade);

    // Customer → Budget (one-to-many)
    modelBuilder.Entity<Customer>()
      .HasMany(c => c.Budgets)
      .WithOne(b => b.Customer)
      .HasForeignKey(b => b.CustomerId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // Customer → Order (one-to-many)
    modelBuilder.Entity<Customer>()
      .HasMany(c => c.Orders)
      .WithOne(o => o.Customer)
      .HasForeignKey(o => o.CustomerId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // Category → FinalProduct (one-to-many)
    modelBuilder.Entity<Category>()
      .HasMany(c => c.FinalProducts)
      .WithOne(fp => fp.Category)
      .HasForeignKey(fp => fp.CategoryId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // Inventory → Supply (one-to-many)
    modelBuilder.Entity<Inventory>()
      .HasMany(i => i.Supplies)
      .WithOne(s => s.Inventory)
      .HasForeignKey(s => s.InventoryId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // FinalProduct → RecipeItem (one-to-many, cascade)
    modelBuilder.Entity<FinalProduct>()
      .HasMany(fp => fp.Recipe)
      .WithOne(r => r.FinalProduct)
      .HasForeignKey(r => r.FinalProductId)
      .OnDelete(DeleteBehavior.Cascade);

    // RecipeItem → Supply (many-to-one)
    modelBuilder.Entity<RecipeItem>()
      .HasOne(r => r.Supply)
      .WithMany()
      .HasForeignKey(r => r.SupplyId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // Receipt → Order
    modelBuilder.Entity<Receipt>()
      .HasOne(r => r.Order)
      .WithMany()
      .HasForeignKey(r => r.OrderId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // Receipt → Customer
    modelBuilder.Entity<Receipt>()
      .HasOne(r => r.Customer)
      .WithMany()
      .HasForeignKey(r => r.CustomerId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // StockMovement → Supply
    modelBuilder.Entity<StockMovement>()
      .HasOne(sm => sm.Supply)
      .WithMany()
      .HasForeignKey(sm => sm.SupplyId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // StockMovement → Order
    modelBuilder.Entity<StockMovement>()
      .HasOne(sm => sm.Order)
      .WithMany()
      .HasForeignKey(sm => sm.OrderId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);

    // Expense → Category (optional FK)
    modelBuilder.Entity<Expense>()
      .HasOne(e => e.Category)
      .WithMany()
      .HasForeignKey(e => e.CategoryId)
      .IsRequired(false)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
