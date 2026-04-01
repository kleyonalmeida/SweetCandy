using Application.Features.MonthlyGoals;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.MonthlyGoals;

public class MonthlyGoalService(AppDbContext context) : IMonthlyGoalService
{
  private readonly AppDbContext _context = context;

  public async Task<MonthlyGoal?> GetByMonthAsync(int year, int month)
    => await _context.MonthlyGoals
      .FirstOrDefaultAsync(g => g.Year == year && g.Month == month);

  public async Task<List<MonthlyGoal>> GetAllAsync()
    => await _context.MonthlyGoals.ToListAsync();

  public async Task<string> UpsertAsync(MonthlyGoal goal)
  {
    var existing = await _context.MonthlyGoals
      .FirstOrDefaultAsync(g => g.Year == goal.Year && g.Month == goal.Month);

    if (existing is not null)
    {
      existing.TargetAmount = goal.TargetAmount;
      existing.UpdatedAt = DateTime.UtcNow;
      await _context.SaveChangesAsync();
      return string.Empty;
    }

    goal.Id = Guid.NewGuid().ToString();
    goal.CreatedAt = DateTime.UtcNow;
    goal.UpdatedAt = DateTime.UtcNow;
    _context.MonthlyGoals.Add(goal);
    await _context.SaveChangesAsync();
    return string.Empty;
  }
}
