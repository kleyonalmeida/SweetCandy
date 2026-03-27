namespace Application.Features.Dashboard.DTOs;

public class DashboardResponse
{
  public int Year { get; set; }
  public int Month { get; set; }
  public decimal Revenue { get; set; }
  public decimal Expenses { get; set; }
  public decimal Profit { get; set; }
  public decimal? MonthlyGoalTarget { get; set; }
  public decimal? GoalProgressPercent { get; set; }
}
