using System;

namespace Domain.Entities;

public class Expense : BaseEntity
{
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; } = false;
  public decimal? TotalExpense { get; set; }
  public DateTime Date { get; set; } = DateTime.UtcNow;
  public string? Category { get; set; }
}
