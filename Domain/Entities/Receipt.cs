using System;
using Domain.Enums;

namespace Domain.Entities;

public class Receipt : BaseEntity
{
  public DateTime Date { get; set; }
  public string? FinalProductName { get; set; }
  public decimal Amount { get; set; }
  public string? Description { get; set; }
  public FormaPagamento PaymentMethod { get; set; } = FormaPagamento.Dinheiro;
  public string? OrderId { get; set; }
  public Order? Order { get; set; }
  public string? CustomerId { get; set; }
  public Customer? Customer { get; set; }
}