using System;

namespace Application.Features.Expenses.DTOs;

public class CreateExpenseRequest
{
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; }
  public DateTime Date { get; set; } = DateTime.UtcNow;
  public string? Category { get; set; }
}
