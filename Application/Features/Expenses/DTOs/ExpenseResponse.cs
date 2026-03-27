using System;

namespace Application.Features.Expenses.DTOs;

public class ExpenseResponse
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; }
  public DateTime Date { get; set; }
  public string? Category { get; set; }
}
