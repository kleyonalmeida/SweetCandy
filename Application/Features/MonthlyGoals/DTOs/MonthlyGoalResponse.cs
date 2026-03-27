namespace Application.Features.MonthlyGoals.DTOs;

public class MonthlyGoalResponse
{
  public string? Id { get; set; }
  public int Year { get; set; }
  public int Month { get; set; }
  public decimal TargetAmount { get; set; }
}
