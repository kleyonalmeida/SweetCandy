using Application.Features.StockMovements;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.StockMovements;

public class StockMovementService(AppDbContext context) : IStockMovementService
{
  private readonly AppDbContext _context = context;

  public async Task<string> CreateAsync(StockMovement movement)
  {
    // Atualiza o saldo do insumo antes de salvar a movimentação
    if (!string.IsNullOrWhiteSpace(movement.SupplyId))
    {
      var supply = await _context.Supplies.FindAsync(movement.SupplyId);
      if (supply is null)
        return $"Insumo '{movement.SupplyId}' não encontrado.";

      var currentQty = supply.Quantity ?? 0m;

      if (movement.Type == MovementType.Saida)
      {
        if (currentQty < movement.Quantity)
          return $"Estoque insuficiente. Disponível: {currentQty}, solicitado: {movement.Quantity}.";
        supply.Quantity = currentQty - movement.Quantity;
      }
      else
      {
        supply.Quantity = currentQty + movement.Quantity;
      }

      supply.UpdatedAt = DateTime.UtcNow;
    }

    movement.Id = Guid.NewGuid().ToString();
    movement.Date = DateTime.UtcNow;
    movement.CreatedAt = DateTime.UtcNow;
    movement.UpdatedAt = DateTime.UtcNow;
    _context.StockMovements.Add(movement);
    await _context.SaveChangesAsync();
    return movement.Id;
  }

  public async Task<StockMovement?> GetByIdAsync(string id)
    => await _context.StockMovements.FindAsync(id);

  public async Task<List<StockMovement>> GetAllAsync()
    => await _context.StockMovements.ToListAsync();

  public async Task<List<StockMovement>> GetBySupplyIdAsync(string supplyId)
    => await _context.StockMovements
      .Where(m => m.SupplyId == supplyId)
      .ToListAsync();

  public async Task<List<StockMovement>> GetByOrderIdAsync(string orderId)
    => await _context.StockMovements
      .Where(m => m.OrderId == orderId)
      .ToListAsync();
}

