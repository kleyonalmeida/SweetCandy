namespace Application.Features.Inventories.DTOs;

public class InventoryResponse
{
  public string? Id { get; set; }
  public decimal? TotalInvested { get; set; }
  public List<SupplyResponse> Supplies { get; set; } = new();
}