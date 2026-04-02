using Domain.Enums;

namespace Application.Features.Expenses.DTOs;

public class ExpenseResponse
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; }
  public decimal? TotalExpense { get; set; }
  public DateTime Date { get; set; }
  public string? Category { get; set; }
  public FormaPagamento PaymentMethod { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
