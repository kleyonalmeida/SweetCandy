using Domain.Enums;

namespace Application.Features.Expenses.DTOs;

public class UpdateExpenseRequest
{
  public string? Name { get; set; }
  public decimal? Value { get; set; }
  public bool? Paid { get; set; }
  public DateTime? Date { get; set; }
  public string? CategoryId { get; set; }
  public string? CategoryName { get; set; }
  public FormaPagamento? PaymentMethod { get; set; }
}
