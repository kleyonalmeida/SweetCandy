namespace Application.Features.Inventories.DTOs;

public class AdjustSupplyStockResponse
{
  public decimal PreviousQuantity { get; set; }
  public decimal NewQuantity { get; set; }
  public string? MovementId { get; set; }
}
