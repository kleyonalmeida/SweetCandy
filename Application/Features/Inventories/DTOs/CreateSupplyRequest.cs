using Domain.Enums;

namespace Application.Features.Inventories.DTOs;

public class CreateSupplyRequest
{
  public string? Name { get; set; }
  public decimal? Quantity { get; set; }
  public decimal? Price { get; set; }
  public Unidade Unit { get; set; } = Unidade.Un;
  public string? InventoryId { get; set; }
}