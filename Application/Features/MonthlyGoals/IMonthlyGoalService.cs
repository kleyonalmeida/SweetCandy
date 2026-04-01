using Domain.Entities;

namespace Application.Features.MonthlyGoals;

public interface IMonthlyGoalService
{
  Task<MonthlyGoal?> GetByMonthAsync(int year, int month);
  Task<List<MonthlyGoal>> GetAllAsync();
  Task<string> UpsertAsync(MonthlyGoal goal);
}
