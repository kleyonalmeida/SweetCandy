using Application.Features.Inventories;
using Domain.Entities;

namespace Infrastructure.Inventories;

public class InventoryService : IInventoryService
{
  private static readonly Inventory Inventory = new();
  private static readonly List<Supply> Supplies = [];
  private static readonly List<FinalProduct> FinalProducts = [];

  public Task<Inventory?> GetInventoryAsync()
  {
    Inventory.Supplies = Supplies.ToList();
    Inventory.TotalInvested = Supplies.Sum(supply => (supply.Price ?? 0m) * (supply.Quantity ?? 0m));
    return Task.FromResult<Inventory?>(Inventory);
  }

  public Task<List<Supply>> GetSuppliesAsync()
  {
    return Task.FromResult(Supplies.ToList());
  }

  public Task<Supply?> GetSupplyByIdAsync(string supplyId)
  {
    return Task.FromResult(Supplies.FirstOrDefault(supply => supply.Id == supplyId));
  }

  public Task<string> CreateSupplyAsync(Supply supply)
  {
    supply.Id = Guid.NewGuid().ToString();
    supply.CreatedAt = DateTime.UtcNow;
    supply.UpdatedAt = DateTime.UtcNow;

    if (string.IsNullOrWhiteSpace(supply.InventoryId))
      supply.InventoryId = Inventory.Id;

    Supplies.Add(supply);
    return Task.FromResult(supply.Id);
  }

  public Task<string> UpdateSupplyAsync(Supply supply)
  {
    var existingSupply = Supplies.FirstOrDefault(currentSupply => currentSupply.Id == supply.Id);

    if (existingSupply is null)
      return Task.FromResult("Insumo nao encontrado.");

    var index = Supplies.IndexOf(existingSupply);
    supply.UpdatedAt = DateTime.UtcNow;
    Supplies[index] = supply;
    return Task.FromResult(string.Empty);
  }

  public Task<string> DeleteSupplyAsync(Supply supply)
  {
    var removed = Supplies.RemoveAll(currentSupply => currentSupply.Id == supply.Id) > 0;
    return Task.FromResult(removed ? string.Empty : "Insumo nao encontrado.");
  }

  public Task<List<FinalProduct>> GetFinalProductsAsync()
  {
    return Task.FromResult(FinalProducts.ToList());
  }

  public Task<FinalProduct?> GetFinalProductByIdAsync(string finalProductId)
  {
    return Task.FromResult(FinalProducts.FirstOrDefault(finalProduct => finalProduct.Id == finalProductId));
  }

  public Task<string> CreateFinalProductAsync(FinalProduct finalProduct)
  {
    finalProduct.Id = Guid.NewGuid().ToString();
    finalProduct.CreatedAt = DateTime.UtcNow;
    finalProduct.UpdatedAt = DateTime.UtcNow;
    FinalProducts.Add(finalProduct);
    return Task.FromResult(finalProduct.Id);
  }

  public Task<string> UpdateFinalProductAsync(FinalProduct finalProduct)
  {
    var existingFinalProduct = FinalProducts.FirstOrDefault(currentFinalProduct => currentFinalProduct.Id == finalProduct.Id);

    if (existingFinalProduct is null)
      return Task.FromResult("Produto final nao encontrado.");

    var index = FinalProducts.IndexOf(existingFinalProduct);
    finalProduct.UpdatedAt = DateTime.UtcNow;
    FinalProducts[index] = finalProduct;
    return Task.FromResult(string.Empty);
  }

  public Task<string> DeleteFinalProductAsync(FinalProduct finalProduct)
  {
    var removed = FinalProducts.RemoveAll(currentFinalProduct => currentFinalProduct.Id == finalProduct.Id) > 0;
    return Task.FromResult(removed ? string.Empty : "Produto final nao encontrado.");
  }
}