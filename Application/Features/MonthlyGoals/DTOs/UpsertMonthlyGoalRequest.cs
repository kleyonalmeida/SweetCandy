namespace Application.Features.MonthlyGoals.DTOs;

public class UpsertMonthlyGoalRequest
{
  public int Year { get; set; }
  public int Month { get; set; }
  public decimal TargetAmount { get; set; }
}
