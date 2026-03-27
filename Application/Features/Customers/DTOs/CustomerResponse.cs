using System;

namespace Application.Features.Customers.DTOs;

public class CustomerResponse
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public string? Email { get; set; }
  public string? Phone { get; set; }
  public string? Address { get; set; }
  public DateTime? BirthDate { get; set; }
}
