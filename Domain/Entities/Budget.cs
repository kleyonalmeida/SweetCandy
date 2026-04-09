using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Budget : BaseEntity
{
  public string? ClientName { get; set; }
  public string? CustomerId { get; set; }
  public Customer? Customer { get; set; }
  public DateTime? EventDate { get; set; }
  public string? FinalProductName { get; set; }
  public string? FinalProductDescription { get; set; }
  public decimal? FinalProductQuantity { get; set; }
  public decimal? FinalUnitPrice { get; set; }
  public decimal? FinalTotalValue { get; set; }

  private readonly List<BudgetItem> _items = new();
  public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();

  public void SetItems(IEnumerable<BudgetItem> items)
  {
    _items.Clear();
    foreach (var item in items)
    {
      item.BudgetId = Id;
      _items.Add(item);
    }
    MarkUpdated();
  }

  public void AddItem(BudgetItem item)
  {
    item.BudgetId = Id;
    _items.Add(item);
    MarkUpdated();
  }

  public void ClearItems() => _items.Clear();
}
