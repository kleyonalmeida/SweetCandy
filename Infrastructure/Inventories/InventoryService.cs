using Application.Features.Inventories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Inventories;

public class InventoryService(AppDbContext context) : IInventoryService
{
  private readonly AppDbContext _context = context;

  // ── Inventory ────────────────────────────────────────────────────────────
  public async Task<Inventory?> GetInventoryAsync()
  {
    var inventory = await _context.Inventories
      .Include(i => i.Supplies)
      .FirstOrDefaultAsync();

    if (inventory is null)
    {
      inventory = new Inventory();
      _context.Inventories.Add(inventory);
      await _context.SaveChangesAsync();
    }

    inventory.TotalInvested = inventory.Supplies
      .Sum(s => (s.Price ?? 0m) * (s.Quantity ?? 0m));

    return inventory;
  }

  private async Task<Inventory> GetOrCreateInventoryAsync()
  {
    var inventory = await _context.Inventories.FirstOrDefaultAsync();
    if (inventory is null)
    {
      inventory = new Inventory();
      _context.Inventories.Add(inventory);
      await _context.SaveChangesAsync();
    }
    return inventory;
  }

  // ── Supplies ──────────────────────────────────────────────────────────────
  public async Task<List<Supply>> GetSuppliesAsync()
    => await _context.Supplies.ToListAsync();

  public async Task<Supply?> GetSupplyByIdAsync(string supplyId)
    => await _context.Supplies.FindAsync(supplyId);

  public async Task<string> CreateSupplyAsync(Supply supply)
  {
    if (string.IsNullOrWhiteSpace(supply.InventoryId))
    {
      var inv = await GetOrCreateInventoryAsync();
      supply.InventoryId = inv.Id;
    }
    _context.Supplies.Add(supply);
    await _context.SaveChangesAsync();
    return supply.Id;
  }

  public async Task<string> UpdateSupplyAsync(Supply supply)
  {
    var existing = await _context.Supplies.FindAsync(supply.Id);
    if (existing is null)
      return "Insumo nao encontrado.";
    existing.Name = supply.Name;
    existing.Quantity = supply.Quantity;
    existing.Price = supply.Price;
    existing.Unit = supply.Unit;
    existing.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteSupplyAsync(Supply supply)
  {
    var existing = await _context.Supplies.FindAsync(supply.Id);
    if (existing is null)
      return "Insumo nao encontrado.";
    _context.Supplies.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  // ── FinalProducts ─────────────────────────────────────────────────────────
  public async Task<List<FinalProduct>> GetFinalProductsAsync()
    => await _context.FinalProducts
      .Include(fp => fp.Recipe)
      .ToListAsync();

  public async Task<FinalProduct?> GetFinalProductByIdAsync(string finalProductId)
    => await _context.FinalProducts
      .Include(fp => fp.Recipe)
        .ThenInclude(r => r.Supply)
      .FirstOrDefaultAsync(fp => fp.Id == finalProductId);

  public async Task<string> CreateFinalProductAsync(FinalProduct finalProduct)
  {
    _context.FinalProducts.Add(finalProduct);
    await _context.SaveChangesAsync();
    return finalProduct.Id;
  }

  public async Task<string> UpdateFinalProductAsync(FinalProduct finalProduct)
  {
    var existing = await _context.FinalProducts.FindAsync(finalProduct.Id);
    if (existing is null)
      return "Produto final nao encontrado.";
    existing.Name = finalProduct.Name;
    existing.Description = finalProduct.Description;
    existing.CategoryId = finalProduct.CategoryId;
    existing.CostPrice = finalProduct.CostPrice;
    existing.UnitPrice = finalProduct.UnitPrice;
    existing.QuantityAvailable = finalProduct.QuantityAvailable;
    existing.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  public async Task<string> DeleteFinalProductAsync(FinalProduct finalProduct)
  {
    var existing = await _context.FinalProducts.FindAsync(finalProduct.Id);
    if (existing is null)
      return "Produto final nao encontrado.";
    _context.FinalProducts.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }

  // ── Recipe ────────────────────────────────────────────────────────────────
  public async Task<List<RecipeItem>> GetRecipeAsync(string finalProductId)
    => await _context.RecipeItems
      .Include(r => r.Supply)
      .Where(r => r.FinalProductId == finalProductId)
      .ToListAsync();

  public async Task<RecipeItem?> GetRecipeItemByIdAsync(string recipeItemId)
    => await _context.RecipeItems
      .Include(r => r.Supply)
      .FirstOrDefaultAsync(r => r.Id == recipeItemId);

  public async Task<string> AddRecipeItemAsync(RecipeItem item)
  {
    _context.RecipeItems.Add(item);
    await _context.SaveChangesAsync();
    return item.Id;
  }

  public async Task<string> RemoveRecipeItemAsync(string recipeItemId)
  {
    var existing = await _context.RecipeItems.FindAsync(recipeItemId);
    if (existing is null)
      return "Item da receita nao encontrado.";
    _context.RecipeItems.Remove(existing);
    await _context.SaveChangesAsync();
    return string.Empty;
  }
}
