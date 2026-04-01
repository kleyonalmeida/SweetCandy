using System;
using Domain.Enums;

namespace Application.Features.StockMovements.DTOs;

public class StockMovementResponse
{
  public string? Id { get; set; }
  public DateTime Date { get; set; }
  public string? SupplyId { get; set; }
  public decimal Quantity { get; set; }
  public MovementType Type { get; set; }
  public string? OrderId { get; set; }
  public string? Notes { get; set; }
}
