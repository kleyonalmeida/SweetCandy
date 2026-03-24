using Application.Features.Orders;
using Domain.Entities;

namespace Infrastructure.Orders;

public class OrderService : IOrdersService
{
  private static readonly List<Order> Orders = [];

  public Task<string> CreateAsync(Order order)
  {
    order.Id = Guid.NewGuid().ToString();
    order.CreatedAt = DateTime.UtcNow;
    order.UpdatedAt = DateTime.UtcNow;
    Orders.Add(order);
    return Task.FromResult(order.Id);
  }

  public Task<string> UpdateAsync(Order order)
  {
    var existingOrder = Orders.FirstOrDefault(currentOrder => currentOrder.Id == order.Id);

    if (existingOrder is null)
      return Task.FromResult("Pedido nao encontrado.");

    var index = Orders.IndexOf(existingOrder);
    order.UpdatedAt = DateTime.UtcNow;
    Orders[index] = order;
    return Task.FromResult(string.Empty);
  }

  public Task<string> DeleteAsync(Order order)
  {
    var removed = Orders.RemoveAll(currentOrder => currentOrder.Id == order.Id) > 0;
    return Task.FromResult(removed ? string.Empty : "Pedido nao encontrado.");
  }

  public Task<Order?> GetByIdAsync(string orderId)
  {
    return Task.FromResult(Orders.FirstOrDefault(currentOrder => currentOrder.Id == orderId));
  }

  public Task<List<Order>> GetAllAsync()
  {
    return Task.FromResult(Orders.ToList());
  }
}