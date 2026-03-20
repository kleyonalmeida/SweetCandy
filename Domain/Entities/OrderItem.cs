namespace Domain.Entities;

public class OrderItem : BaseEntity
{
  public string? FinalProductId { get; set; }
  public string? FinalProductName { get; set; }
  public decimal Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal TotalPrice { get; set; }
  public string? OrderId { get; set; }
  public Order? Order { get; set; }
}