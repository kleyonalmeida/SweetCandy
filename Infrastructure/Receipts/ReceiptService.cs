using Application.Features.Receipts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Receipts;

public class ReceiptService(AppDbContext context) : IReceiptsService
{
  private readonly AppDbContext _context = context;

  public async Task<string> CreateAsync(Receipt receipt)
  {
    _context.Receipts.Add(receipt);
    await _context.SaveChangesAsync();
    return receipt.Id;
  }

  public async Task<string> UpdateAsync(Receipt receipt)
  {
    var existing = await _context.Receipts.FindAsync(receipt.Id);
    if (existing is null)
      return "Recebimento nao encontrado.";
    existing.Date = receipt.Date;
    existing.FinalProductName = receipt.FinalProductName;
    existing.Amount = receipt.Amount;
    existing.Description = receipt.Description;
    existing.PaymentMethod = receipt.PaymentMethod;
    existing.OrderId = receipt.OrderId;
    existing.CustomerId = receipt.CustomerId;
    existing.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteAsync(Receipt receipt)
  {
    var existing = await _context.Receipts.FindAsync(receipt.Id);
    if (existing is null)
      return "Recebimento nao encontrado.";
    _context.Receipts.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<Receipt?> GetByIdAsync(string receiptId)
    => await _context.Receipts.FindAsync(receiptId);

  public async Task<List<Receipt>> GetAllAsync()
    => await _context.Receipts.ToListAsync();
}
