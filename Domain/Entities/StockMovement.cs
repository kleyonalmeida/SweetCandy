using System;
using Domain.Enums;

namespace Domain.Entities;

public class StockMovement : BaseEntity
{
  public DateTime Date { get; set; } = DateTime.UtcNow;
  public string? SupplyId { get; set; }
  public Supply? Supply { get; set; }
  public decimal Quantity { get; set; }
  public MovementType Type { get; set; } = MovementType.Saida;
  public string? OrderId { get; set; }
  public Order? Order { get; set; }
  public string? Notes { get; set; }
}