namespace Application.Features.Budget.DTOs;

public class UpdateBudgetRequest
{
  public string? ClientName { get; set; }
  public DateTime? EventDate { get; set; }
  public string? FinalProductName { get; set; }
  public string? FinalProductDescription { get; set; }
  public decimal? FinalProductQuantity { get; set; }
  public decimal? FinalUnitPrice { get; set; }
  public decimal? FinalTotalValue { get; set; }
}
