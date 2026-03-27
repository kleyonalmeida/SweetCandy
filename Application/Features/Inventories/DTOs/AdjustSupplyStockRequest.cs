namespace Application.Features.Inventories.DTOs;

public class AdjustSupplyStockRequest
{
  public decimal Quantity { get; set; }
  public bool Add { get; set; }
  public string? Notes { get; set; }
}
