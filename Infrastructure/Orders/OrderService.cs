using Application.Features.Orders;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Orders;

public class OrderService(AppDbContext context) : IOrdersService
{
  private readonly AppDbContext _context = context;

  public async Task<string> CreateAsync(Order order)
  {
    order.Id = Guid.NewGuid().ToString();
    order.CreatedAt = DateTime.UtcNow;
    order.UpdatedAt = DateTime.UtcNow;
    foreach (var item in order.Items)
    {
      item.Id = Guid.NewGuid().ToString();
      item.OrderId = order.Id;
      item.CreatedAt = DateTime.UtcNow;
      item.UpdatedAt = DateTime.UtcNow;
    }
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();
    return order.Id;
  }

  public async Task<string> UpdateAsync(Order order)
  {
    var existing = await _context.Orders
      .Include(o => o.Items)
      .FirstOrDefaultAsync(o => o.Id == order.Id);

    if (existing is null)
      return "Pedido nao encontrado.";

    existing.Name = order.Name;
    existing.CustomerId = order.CustomerId;
    existing.EventDate = order.EventDate;
    existing.Status = order.Status;
    existing.Sinal = order.Sinal;
    existing.TotalValue = order.TotalValue;
    existing.UpdatedAt = DateTime.UtcNow;

    if (order.Items.Count > 0)
    {
      _context.OrderItems.RemoveRange(existing.Items);
      foreach (var item in order.Items)
      {
        item.Id = Guid.NewGuid().ToString();
        item.OrderId = existing.Id;
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        existing.Items.Add(item);
      }
    }

    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteAsync(Order order)
  {
    var existing = await _context.Orders.FindAsync(order.Id);
    if (existing is null)
      return "Pedido nao encontrado.";
    _context.Orders.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<Order?> GetByIdAsync(string orderId)
    => await _context.Orders
      .Include(o => o.Items)
      .FirstOrDefaultAsync(o => o.Id == orderId);

  public async Task<List<Order>> GetAllAsync()
    => await _context.Orders
      .Include(o => o.Items)
      .ToListAsync();
}
