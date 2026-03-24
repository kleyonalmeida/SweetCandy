using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities;

public class Order : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? CustomerId { get; set; }
  public Customer? Customer { get; set; }
  public DateTime? EventDate { get; set; }
  public StatusOrder Status { get; set; } = StatusOrder.Pendente;
  public int? Sinal { get; set; }
  public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
  public decimal? TotalValue { get; set; }
}