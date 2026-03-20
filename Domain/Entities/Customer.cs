using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Customer : BaseEntity
{
  public string? Name { get; set; }
  public string? Email { get; set; }
  public string? Phone { get; set; }
  public string? Address { get; set; }
  public DateTime? BirthDate { get; set; }
  public List<Budget> Budgets { get; set; } = new List<Budget>();
  public List<Order> Orders { get; set; } = new List<Order>();

}