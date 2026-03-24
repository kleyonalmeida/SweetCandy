using Domain.Enums;

namespace Application.Features.Orders.DTOs;

public class CreateOrderRequest
{
  public string? Name { get; set; }
  public string? CustomerId { get; set; }
  public DateTime? EventDate { get; set; }
  public StatusOrder Status { get; set; } = StatusOrder.Pendente;
  public int? Sinal { get; set; }
  public decimal? TotalValue { get; set; }
  public List<CreateOrderItemRequest> Items { get; set; } = new();
}