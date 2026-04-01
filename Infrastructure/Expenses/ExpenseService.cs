using Application.Features.Expenses;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Expenses;

public class ExpenseService(AppDbContext context) : IExpenseService
{
  private readonly AppDbContext _context = context;

  public async Task<string> CreateAsync(Expense expense)
  {
    expense.Id = Guid.NewGuid().ToString();
    expense.CreatedAt = DateTime.UtcNow;
    expense.UpdatedAt = DateTime.UtcNow;
    _context.Expenses.Add(expense);
    await _context.SaveChangesAsync();
    return expense.Id;
  }

  public async Task<string> UpdateAsync(Expense expense)
  {
    var existing = await _context.Expenses.FindAsync(expense.Id);
    if (existing is null)
      return "Despesa nao encontrada.";
    existing.Name = expense.Name;
    existing.Value = expense.Value;
    existing.Paid = expense.Paid;
    existing.TotalExpense = expense.TotalExpense;
    existing.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteAsync(Expense expense)
  {
    var existing = await _context.Expenses.FindAsync(expense.Id);
    if (existing is null)
      return "Despesa nao encontrada.";
    _context.Expenses.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<Expense?> GetByIdAsync(string expenseId)
    => await _context.Expenses.FindAsync(expenseId);

  public async Task<List<Expense>> GetAllAsync()
    => await _context.Expenses.ToListAsync();
}

