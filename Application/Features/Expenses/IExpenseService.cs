using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.Expenses;

public interface IExpenseService
{
  Task<string> CreateAsync(Expense expense);
  Task<string> UpdateAsync(Expense expense);
  Task<string> DeleteAsync(Expense expense);
  Task<Expense?> GetByIdAsync(string expenseId);
  Task<List<Expense>> GetAllAsync();
}
