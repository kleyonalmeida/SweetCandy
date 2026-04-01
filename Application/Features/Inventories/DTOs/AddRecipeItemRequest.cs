using Domain.Enums;

namespace Application.Features.Inventories.DTOs;

public class AddRecipeItemRequest
{
  public string? SupplyId { get; set; }
  public decimal Quantity { get; set; }
  public Unidade Unit { get; set; }
}
