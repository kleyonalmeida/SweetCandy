namespace Application.Features.Orders.DTOs;

public class CreateOrderItemRequest
{
  public string? FinalProductId { get; set; }
  public string? FinalProductName { get; set; }
  public decimal Quantity { get; set; }
  public decimal? UnitPrice { get; set; }
}