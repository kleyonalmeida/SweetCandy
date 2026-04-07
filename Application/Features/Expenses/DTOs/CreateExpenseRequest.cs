using Domain.Enums;

namespace Application.Features.Expenses.DTOs;

public class CreateExpenseRequest
{
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool Paid { get; set; } = false;
  public DateTime Date { get; set; } = DateTime.UtcNow;
  public string? CategoryId { get; set; }
  public string? CategoryName { get; set; }
  public FormaPagamento PaymentMethod { get; set; } = FormaPagamento.Dinheiro;
}
