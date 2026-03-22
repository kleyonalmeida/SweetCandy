namespace Application.Features.Budgets.DTOs;

public class CreateBudgetItemRequest
{
  public string? FinalProductId { get; set; }
  public string? FinalProductName { get; set; }
  public decimal Quantity { get; set; }
  public decimal? UnitPrice { get; set; }
}