using Domain.Enums;

namespace Application.Features.Inventories.DTOs;

public class RecipeItemResponse
{
  public string? Id { get; set; }
  public string? FinalProductId { get; set; }
  public string? SupplyId { get; set; }
  public string? SupplyName { get; set; }
  public decimal Quantity { get; set; }
  public Unidade Unit { get; set; }
}
