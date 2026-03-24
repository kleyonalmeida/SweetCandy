using Application.Features.Receipts;
using Domain.Entities;

namespace Infrastructure.Receipts;

public class ReceiptService : IReceiptsService
{
  private static readonly List<Receipt> Receipts = [];

  public Task<string> CreateAsync(Receipt receipt)
  {
    receipt.Id = Guid.NewGuid().ToString();
    receipt.CreatedAt = DateTime.UtcNow;
    receipt.UpdatedAt = DateTime.UtcNow;
    Receipts.Add(receipt);
    return Task.FromResult(receipt.Id);
  }

  public Task<string> UpdateAsync(Receipt receipt)
  {
    var existingReceipt = Receipts.FirstOrDefault(currentReceipt => currentReceipt.Id == receipt.Id);

    if (existingReceipt is null)
      return Task.FromResult("Recebimento nao encontrado.");

    var index = Receipts.IndexOf(existingReceipt);
    receipt.UpdatedAt = DateTime.UtcNow;
    Receipts[index] = receipt;
    return Task.FromResult(string.Empty);
  }

  public Task<string> DeleteAsync(Receipt receipt)
  {
    var removed = Receipts.RemoveAll(currentReceipt => currentReceipt.Id == receipt.Id) > 0;
    return Task.FromResult(removed ? string.Empty : "Recebimento nao encontrado.");
  }

  public Task<Receipt?> GetByIdAsync(string receiptId)
  {
    return Task.FromResult(Receipts.FirstOrDefault(currentReceipt => currentReceipt.Id == receiptId));
  }

  public Task<List<Receipt>> GetAllAsync()
  {
    return Task.FromResult(Receipts.ToList());
  }
}