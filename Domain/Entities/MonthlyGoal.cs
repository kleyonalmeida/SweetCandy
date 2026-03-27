using System;

namespace Domain.Entities;

public class MonthlyGoal : BaseEntity
{
  public int Year { get; set; }
  public int Month { get; set; }
  public decimal TargetAmount { get; set; }
}
