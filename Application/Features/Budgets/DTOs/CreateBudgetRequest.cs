<<<<<<< HEAD
=======
using System.Collections.Generic;
using Domain.Entities;

>>>>>>> 6999f401069930cbfffaed07213b0a8c214414cf
namespace Application.Features.Budgets.DTOs;

public class CreateBudgetRequest
{
  public string? ClientName { get; set; }
  public DateTime? EventDate { get; set; }
  public string? FinalProductName { get; set; }
  public string? FinalProductDescription { get; set; }
  public decimal? FinalProductQuantity { get; set; }
  public decimal? FinalUnitPrice { get; set; }
  public decimal? FinalTotalValue { get; set; }
<<<<<<< HEAD
=======
  public List<BudgetItem>? Items { get; set; }
>>>>>>> 6999f401069930cbfffaed07213b0a8c214414cf
}
