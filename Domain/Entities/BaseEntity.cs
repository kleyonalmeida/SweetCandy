using System;

namespace Domain.Entities;

public class BaseEntity
{
  public string Id { get; init; } = Guid.NewGuid().ToString();
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
}
