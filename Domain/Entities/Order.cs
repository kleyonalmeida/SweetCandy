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
  public decimal? Sinal { get; set; }
  public decimal? TotalValue { get; set; }

  private readonly List<OrderItem> _items = new();
  public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

  public void SetItems(IEnumerable<OrderItem> items)
  {
    _items.Clear();
    foreach (var item in items)
    {
      item.OrderId = Id;
      _items.Add(item);
    }
    MarkUpdated();
  }

  public void AddItem(OrderItem item)
  {
    item.OrderId = Id;
    _items.Add(item);
    MarkUpdated();
  }

  public void ClearItems() => _items.Clear();
}