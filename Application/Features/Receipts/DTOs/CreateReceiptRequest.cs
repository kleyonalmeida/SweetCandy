using Domain.Enums;

namespace Application.Features.Receipts.DTOs;

public class CreateReceiptRequest
{
  public DateTime Date { get; set; }
  public string? FinalProductName { get; set; }
  public decimal Amount { get; set; }
  public string? Description { get; set; }
  public FormaPagamento PaymentMethod { get; set; } = FormaPagamento.Dinheiro;
  public string? OrderId { get; set; }
  public string? CustomerId { get; set; }
}