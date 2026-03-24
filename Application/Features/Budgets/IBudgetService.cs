using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.Budgets;

public interface IBudgetService
{
  Task<string> CreateAsync(Budget budget);
  Task<string> UpdateAsync(Budget budget);
  Task<string> DeleteAsync(Budget budget);
  Task<Budget> GetByIdAsync(string budgetId);
  Task<List<Budget>> GetAllAsync();
}
