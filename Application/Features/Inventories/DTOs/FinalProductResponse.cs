namespace Application.Features.Inventories.DTOs;

public class FinalProductResponse
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? CategoryId { get; set; }
  public decimal? CostPrice { get; set; }
  public decimal? UnitPrice { get; set; }
  public decimal? QuantityAvailable { get; set; }
}