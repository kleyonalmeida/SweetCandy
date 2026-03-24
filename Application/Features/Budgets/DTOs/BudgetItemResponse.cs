namespace Application.Features.Budgets.DTOs;

public class BudgetItemResponse
{
  public string? Id { get; set; }
  public string? FinalProductId { get; set; }
  public string? FinalProductName { get; set; }
  public decimal Quantity { get; set; }
  public decimal? UnitPrice { get; set; }
  public decimal? TotalPrice { get; set; }
}