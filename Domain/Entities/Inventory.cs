using System.Collections.Generic;

namespace Domain.Entities;

public class Inventory : BaseEntity
{
  public List<Supply> Supplies { get; set; } = new List<Supply>();
  public decimal? TotalInvested { get; set; }
}
