namespace Application.Features.Expenses.DTOs;

public class CreateExpenseRequest
{
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; } = false;
}
