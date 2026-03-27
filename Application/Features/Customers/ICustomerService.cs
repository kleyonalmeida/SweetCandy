using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.Customers;

public interface ICustomerService
{
  Task<string> CreateAsync(Customer customer);
  Task<string> UpdateAsync(Customer customer);
  Task<string> DeleteAsync(Customer customer);
  Task<Customer?> GetByIdAsync(string customerId);
  Task<List<Customer>> GetAllAsync();
}
