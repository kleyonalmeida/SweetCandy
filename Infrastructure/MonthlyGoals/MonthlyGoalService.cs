using Application.Features.MonthlyGoals;
using Domain.Entities;

namespace Infrastructure.MonthlyGoals;

public class MonthlyGoalService : IMonthlyGoalService
{
  private static readonly List<MonthlyGoal> Goals = [];

  public Task<MonthlyGoal?> GetByMonthAsync(int year, int month)
  {
    return Task.FromResult(Goals.FirstOrDefault(g => g.Year == year && g.Month == month));
  }

  public Task<List<MonthlyGoal>> GetAllAsync()
  {
    return Task.FromResult(Goals.ToList());
  }

  public Task<string> UpsertAsync(MonthlyGoal goal)
  {
    var existing = Goals.FirstOrDefault(g => g.Year == goal.Year && g.Month == goal.Month);
    if (existing is not null)
    {
      existing.TargetAmount = goal.TargetAmount;
      existing.UpdatedAt = DateTime.UtcNow;
      return Task.FromResult(string.Empty);
    }

    goal.Id = Guid.NewGuid().ToString();
    goal.CreatedAt = DateTime.UtcNow;
    goal.UpdatedAt = DateTime.UtcNow;
    Goals.Add(goal);
    return Task.FromResult(string.Empty);
  }
}
