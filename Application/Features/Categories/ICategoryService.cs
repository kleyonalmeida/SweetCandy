using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.Categories;

public interface ICategoryService
{
  Task<string> CreateAsync(Category category);
  Task<string> UpdateAsync(Category category);
  Task<string> DeleteAsync(Category category);
  Task<Category?> GetByIdAsync(string categoryId);
  Task<List<Category>> GetAllAsync();
}
