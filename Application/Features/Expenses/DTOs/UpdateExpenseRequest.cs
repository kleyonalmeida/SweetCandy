using System;

namespace Application.Features.Expenses.DTOs;

public class UpdateExpenseRequest
{
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; }
  public DateTime Date { get; set; } = DateTime.UtcNow;
  public string? Category { get; set; }
}
