using Domain.Entities;

namespace Application.Features.Receipts;

public interface IReceiptsService
{
  Task<string> CreateAsync(Receipt receipt);
  Task<string> UpdateAsync(Receipt receipt);
  Task<string> DeleteAsync(Receipt receipt);
  Task<Receipt?> GetByIdAsync(string receiptId);
  Task<List<Receipt>> GetAllAsync();
}