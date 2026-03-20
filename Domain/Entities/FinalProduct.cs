using System.Collections.Generic;

namespace Domain.Entities;

public class FinalProduct : BaseEntity
{
  public string? Name { get; set; }
  public string? Description { get; set; }
  public string? CategoryId { get; set; }
  public Category? Category { get; set; }
  public decimal? CostPrice { get; set; }
  public decimal? UnitPrice { get; set; }
  public decimal? QuantityAvailable { get; set; }
  public List<Supply> Recipe { get; set; } = new List<Supply>();
}
