using Application.Features.Customers;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Customers;

public class CustomerService(AppDbContext context) : ICustomerService
{
  private readonly AppDbContext _context = context;

  public async Task<string> CreateAsync(Customer customer)
  {
    _context.Customers.Add(customer);
    await _context.SaveChangesAsync();
    return customer.Id;
  }

  public async Task<string> UpdateAsync(Customer customer)
  {
    var existing = await _context.Customers.FindAsync(customer.Id);
    if (existing is null)
      return "Cliente nao encontrado.";
    existing.Name = customer.Name;
    existing.Email = customer.Email;
    existing.Phone = customer.Phone;
    existing.Address = customer.Address;
    existing.BirthDate = customer.BirthDate;
    existing.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteAsync(Customer customer)
  {
    var existing = await _context.Customers.FindAsync(customer.Id);
    if (existing is null)
      return "Cliente nao encontrado.";
    _context.Customers.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<Customer?> GetByIdAsync(string customerId)
    => await _context.Customers.FindAsync(customerId);

  public async Task<List<Customer>> GetAllAsync()
    => await _context.Customers.ToListAsync();
}

