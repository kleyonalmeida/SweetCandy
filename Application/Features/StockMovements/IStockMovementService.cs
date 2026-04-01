using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.StockMovements;

public interface IStockMovementService
{
  Task<string> CreateAsync(StockMovement movement);
  Task<StockMovement?> GetByIdAsync(string id);
  Task<List<StockMovement>> GetAllAsync();
  Task<List<StockMovement>> GetBySupplyIdAsync(string supplyId);
  Task<List<StockMovement>> GetByOrderIdAsync(string orderId);
}
