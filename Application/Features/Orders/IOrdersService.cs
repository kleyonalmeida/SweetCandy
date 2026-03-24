using Domain.Entities;

namespace Application.Features.Orders;

public interface IOrdersService
{
  Task<string> CreateAsync(Order order);
  Task<string> UpdateAsync(Order order);
  Task<string> DeleteAsync(Order order);
  Task<Order?> GetByIdAsync(string orderId);
  Task<List<Order>> GetAllAsync();
}