using System;

namespace Domain.Entities;

public class Budget : BaseEntity
{
  public string? ClientName { get; set; }
  public string? CustomerId { get; set; }
  public Customer? Customer { get; set; }
  public DateTime? EventDate { get; set; }
  public string? FinalProductName { get; set; }
  public string? FinalProductDescription { get; set; }
  public decimal? FinalProductQuantity { get; set; }
  public decimal? FinalUnitPrice { get; set; }
  public decimal? FinalTotalValue { get; set; }

}
