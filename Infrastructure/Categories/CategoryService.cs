using Application.Features.Categories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Categories;

public class CategoryService(AppDbContext context) : ICategoryService
{
  private readonly AppDbContext _context = context;

  public async Task<string> CreateAsync(Category category)
  {
    _context.Categories.Add(category);
    await _context.SaveChangesAsync();
    return category.Id;
  }

  public async Task<string> UpdateAsync(Category category)
  {
    var existing = await _context.Categories.FindAsync(category.Id);
    if (existing is null)
      return "Categoria nao encontrada.";
    existing.Name = category.Name;
    existing.Description = category.Description;
    existing.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteAsync(Category category)
  {
    var existing = await _context.Categories.FindAsync(category.Id);
    if (existing is null)
      return "Categoria nao encontrada.";
    _context.Categories.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<Category?> GetByIdAsync(string categoryId)
    => await _context.Categories.FindAsync(categoryId);

  public async Task<List<Category>> GetAllAsync()
    => await _context.Categories.ToListAsync();
}

