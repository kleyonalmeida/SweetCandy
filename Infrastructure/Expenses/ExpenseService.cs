using Application.Features.Expenses;
using Domain.Entities;

namespace Infrastructure.Expenses;

public class ExpenseService : IExpenseService
{
  private static readonly List<Expense> Expenses = [];

  public Task<string> CreateAsync(Expense expense)
  {
    expense.Id = Guid.NewGuid().ToString();
    expense.CreatedAt = DateTime.UtcNow;
    expense.UpdatedAt = DateTime.UtcNow;
    if (expense.Date == default)
      expense.Date = DateTime.UtcNow;
    Expenses.Add(expense);
    return Task.FromResult(expense.Id);
  }

  public Task<string> UpdateAsync(Expense expense)
  {
    var existing = Expenses.FirstOrDefault(e => e.Id == expense.Id);
    if (existing is null)
      return Task.FromResult("Despesa nao encontrada.");

    var index = Expenses.IndexOf(existing);
    expense.UpdatedAt = DateTime.UtcNow;
    expense.CreatedAt = existing.CreatedAt;
    Expenses[index] = expense;
    return Task.FromResult(string.Empty);
  }

  public Task<string> DeleteAsync(Expense expense)
  {
    var removed = Expenses.RemoveAll(e => e.Id == expense.Id) > 0;
    return Task.FromResult(removed ? string.Empty : "Despesa nao encontrada.");
  }

  public Task<Expense?> GetByIdAsync(string expenseId)
  {
    return Task.FromResult(Expenses.FirstOrDefault(e => e.Id == expenseId));
  }

  public Task<List<Expense>> GetAllAsync()
  {
    return Task.FromResult(Expenses.ToList());
  }
}
