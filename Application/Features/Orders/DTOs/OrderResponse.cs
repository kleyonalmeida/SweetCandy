using Domain.Enums;

namespace Application.Features.Orders.DTOs;

public class OrderResponse
{
  public string? Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? CustomerId { get; set; }
  public DateTime? EventDate { get; set; }
  public StatusOrder Status { get; set; }
  public decimal? Sinal { get; set; }
  public decimal? TotalValue { get; set; }
  public List<OrderItemResponse> Items { get; set; } = new();
}