using Application.Features.Inventories.DTOs;
using Domain.Entities;

namespace Application.Features.Inventories;

public interface IInventoryService
{
  Task<Inventory?> GetInventoryAsync();
  Task<List<Supply>> GetSuppliesAsync();
  Task<Supply?> GetSupplyByIdAsync(string supplyId);
  Task<string> CreateSupplyAsync(Supply supply);
  Task<string> UpdateSupplyAsync(Supply supply);
  Task<string> DeleteSupplyAsync(Supply supply);

  Task<List<FinalProduct>> GetFinalProductsAsync();
  Task<FinalProduct?> GetFinalProductByIdAsync(string finalProductId);
  Task<string> CreateFinalProductAsync(FinalProduct finalProduct);
  Task<string> UpdateFinalProductAsync(FinalProduct finalProduct);
  Task<string> DeleteFinalProductAsync(FinalProduct finalProduct);

  Task<(string? Error, AdjustSupplyStockResponse? Result)> AdjustSupplyStockAsync(string supplyId, decimal quantity, bool add, string? notes);
  Task<List<StockMovement>> GetStockMovementsAsync(string? supplyId);
}