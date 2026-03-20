using Domain.Entities;

namespace Application.Features.Budget;

public interface IBudgetService
{
  Task<string> CreateAsync(Budget budget);
  Task<string> UpdateAsync(Budget budget);
  Task<string> DeleteAsync(Budget budget);
  Task<Budget> GetByIdAsync(string budgetId);
  Task<List<Budget>> GetAllAsync();
}
