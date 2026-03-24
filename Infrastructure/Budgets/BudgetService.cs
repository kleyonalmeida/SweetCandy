using Application.Features.Budgets;
using Domain.Entities;

namespace Infrastructure.Budgets;

public class BudgetService : IBudgetService
{
  private static readonly List<Budget> Budgets = [];

  public Task<string> CreateAsync(Budget budget)
  {
    budget.Id = Guid.NewGuid().ToString();
    budget.CreatedAt = DateTime.UtcNow;
    budget.UpdatedAt = DateTime.UtcNow;
    Budgets.Add(budget);
    return Task.FromResult(budget.Id);
  }

  public Task<string> UpdateAsync(Budget budget)
  {
    var existingBudget = Budgets.FirstOrDefault(currentBudget => currentBudget.Id == budget.Id);

    if (existingBudget is null)
      return Task.FromResult("Orcamento nao encontrado.");

    var index = Budgets.IndexOf(existingBudget);
    budget.UpdatedAt = DateTime.UtcNow;
    Budgets[index] = budget;
    return Task.FromResult(string.Empty);
  }

  public Task<string> DeleteAsync(Budget budget)
  {
    var removed = Budgets.RemoveAll(currentBudget => currentBudget.Id == budget.Id) > 0;
    return Task.FromResult(removed ? string.Empty : "Orcamento nao encontrado.");
  }

  public Task<Budget> GetByIdAsync(string budgetId)
  {
    var budget = Budgets.FirstOrDefault(currentBudget => currentBudget.Id == budgetId);
    return Task.FromResult(budget!);
  }

  public Task<List<Budget>> GetAllAsync()
  {
    return Task.FromResult(Budgets.ToList());
  }
}