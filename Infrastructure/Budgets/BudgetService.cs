using Application.Features.Budgets;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Budgets;

public class BudgetService(AppDbContext context) : IBudgetService
{
  private readonly AppDbContext _context = context;

  public async Task<string> CreateAsync(Budget budget)
  {
    budget.Id = Guid.NewGuid().ToString();
    budget.CreatedAt = DateTime.UtcNow;
    budget.UpdatedAt = DateTime.UtcNow;
    foreach (var item in budget.Items)
    {
      item.Id = Guid.NewGuid().ToString();
      item.BudgetId = budget.Id;
      item.CreatedAt = DateTime.UtcNow;
      item.UpdatedAt = DateTime.UtcNow;
    }
    _context.Budgets.Add(budget);
    await _context.SaveChangesAsync();
    return budget.Id;
  }

  public async Task<string> UpdateAsync(Budget budget)
  {
    var existing = await _context.Budgets
      .Include(b => b.Items)
      .FirstOrDefaultAsync(b => b.Id == budget.Id);

    if (existing is null)
      return "Orcamento nao encontrado.";

    existing.ClientName = budget.ClientName;
    existing.CustomerId = budget.CustomerId;
    existing.EventDate = budget.EventDate;
    existing.FinalProductName = budget.FinalProductName;
    existing.FinalProductDescription = budget.FinalProductDescription;
    existing.FinalProductQuantity = budget.FinalProductQuantity;
    existing.FinalUnitPrice = budget.FinalUnitPrice;
    existing.FinalTotalValue = budget.FinalTotalValue;
    existing.UpdatedAt = DateTime.UtcNow;

    if (budget.Items.Count > 0)
    {
      _context.BudgetItems.RemoveRange(existing.Items);
      foreach (var item in budget.Items)
      {
        item.Id = Guid.NewGuid().ToString();
        item.BudgetId = existing.Id;
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        existing.Items.Add(item);
      }
    }

    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteAsync(Budget budget)
  {
    var existing = await _context.Budgets.FindAsync(budget.Id);
    if (existing is null)
      return "Orcamento nao encontrado.";
    _context.Budgets.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<Budget?> GetByIdAsync(string budgetId)
    => await _context.Budgets
      .Include(b => b.Items)
      .FirstOrDefaultAsync(b => b.Id == budgetId);

  public async Task<List<Budget>> GetAllAsync()
    => await _context.Budgets
      .Include(b => b.Items)
      .ToListAsync();
}
