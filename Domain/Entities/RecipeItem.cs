using Domain.Enums;

namespace Domain.Entities;

public class RecipeItem : BaseEntity
{
  public string? FinalProductId { get; set; }
  public FinalProduct? FinalProduct { get; set; }
  public string? SupplyId { get; set; }
  public Supply? Supply { get; set; }
  public decimal Quantity { get; set; }
  public Unidade Unit { get; set; } = Unidade.Un;
}
