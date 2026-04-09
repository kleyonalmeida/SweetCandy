using System;
using Domain.Enums;

namespace Domain.Entities;

public class Expense : BaseEntity
{
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; } = false;
  public decimal? TotalExpense { get; set; }
  public DateTime Date { get; set; } = DateTime.UtcNow;
  /// <summary>Texto livre legado — preferência: use CategoryId (FK).</summary>
  public string? CategoryName { get; set; }
  public string? CategoryId { get; set; }
  public Category? Category { get; set; }
  public FormaPagamento PaymentMethod { get; set; } = FormaPagamento.Dinheiro;

  public void MarkAsPaid()
  {
    if (Paid) throw new InvalidOperationException("Despesa já está paga.");
    Paid = true;
    MarkUpdated();
  }
}
