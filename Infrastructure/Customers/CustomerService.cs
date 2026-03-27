using Application.Features.Customers;
using Domain.Entities;

namespace Infrastructure.Customers;

public class CustomerService : ICustomerService
{
  private static readonly List<Customer> Customers = [];

  public Task<string> CreateAsync(Customer customer)
  {
    customer.Id = Guid.NewGuid().ToString();
    customer.CreatedAt = DateTime.UtcNow;
    customer.UpdatedAt = DateTime.UtcNow;
    Customers.Add(customer);
    return Task.FromResult(customer.Id);
  }

  public Task<string> UpdateAsync(Customer customer)
  {
    var existing = Customers.FirstOrDefault(c => c.Id == customer.Id);
    if (existing is null)
      return Task.FromResult("Cliente nao encontrado.");

    var index = Customers.IndexOf(existing);
    customer.UpdatedAt = DateTime.UtcNow;
    customer.CreatedAt = existing.CreatedAt;
    Customers[index] = customer;
    return Task.FromResult(string.Empty);
  }

  public Task<string> DeleteAsync(Customer customer)
  {
    var removed = Customers.RemoveAll(c => c.Id == customer.Id) > 0;
    return Task.FromResult(removed ? string.Empty : "Cliente nao encontrado.");
  }

  public Task<Customer?> GetByIdAsync(string customerId)
  {
    return Task.FromResult(Customers.FirstOrDefault(c => c.Id == customerId));
  }

  public Task<List<Customer>> GetAllAsync()
  {
    return Task.FromResult(Customers.ToList());
  }
}
