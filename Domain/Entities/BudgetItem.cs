namespace Domain.Entities;

public class BudgetItem : BaseEntity
{
  public string? BudgetId { get; set; }
  public Budget? Budget { get; set; }
  public string? FinalProductId { get; set; }
  public string? FinalProductName { get; set; }
  public decimal Quantity { get; set; }
  public decimal? UnitPrice { get; set; }
  public decimal? TotalPrice => UnitPrice.HasValue ? UnitPrice.Value * Quantity : null;
}