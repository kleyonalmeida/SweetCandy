using System.Collections.Generic;
using Domain.Entities;

namespace Application.Features.Budgets.DTOs;

public class BudgetResponse
{
  public string? Id { get; set; }
  public string? ClientName { get; set; }
  public DateTime? EventDate { get; set; }
  public string? FinalProductName { get; set; }
  public string? FinalProductDescription { get; set; }
  public decimal? FinalProductQuantity { get; set; }
  public decimal? FinalUnitPrice { get; set; }
  public decimal? FinalTotalValue { get; set; }
  public List<BudgetItem>? Items { get; set; }
}

