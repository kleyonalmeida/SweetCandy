using Domain.Enums;

namespace Application.Features.Inventories.DTOs;

public class SupplyResponse
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public decimal? Quantity { get; set; }
  public decimal? Price { get; set; }
  public Unidade Unit { get; set; }
  public decimal? TotalPrice { get; set; }
  public string? InventoryId { get; set; }
}