using Domain.Enums;

namespace Application.Features.StockMovements.DTOs;

public class CreateStockMovementRequest
{
  public string? SupplyId { get; set; }
  public decimal Quantity { get; set; }
  public MovementType Type { get; set; } = MovementType.Entrada;
  public string? OrderId { get; set; }
  public string? Notes { get; set; }
}
