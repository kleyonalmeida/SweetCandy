using Domain.Enums;

namespace Domain.Entities;

public class Supply : BaseEntity
{
  public string? Name { get; set; }
  public decimal? Quantity { get; set; }
  public decimal? Price { get; set; }
  public Unidade Unit { get; set; } = Unidade.Un;
  public string? InventoryId { get; set; }
  public Inventory? Inventory { get; set; }
}
