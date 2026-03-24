using Domain.Enums;

namespace Application.Features.Receipts.DTOs;

public class UpdateReceiptRequest
{
  public DateTime? Date { get; set; }
  public string? FinalProductName { get; set; }
  public decimal? Amount { get; set; }
  public string? Description { get; set; }
  public FormaPagamento? PaymentMethod { get; set; }
  public string? OrderId { get; set; }
  public string? CustomerId { get; set; }
}