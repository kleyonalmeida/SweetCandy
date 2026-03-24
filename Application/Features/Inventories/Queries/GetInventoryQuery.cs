using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Queries;

public record GetInventoryQuery() : IRequest<ResponseWrapper<InventoryResponse>>;

public class GetInventoryQueryHandler(IInventoryService inventoryService) : IRequestHandler<GetInventoryQuery, ResponseWrapper<InventoryResponse>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<InventoryResponse>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
  {
    var inventory = await _inventoryService.GetInventoryAsync();

    if (inventory is null)
      return await ResponseWrapper<InventoryResponse>.SuccessAsync(new InventoryResponse());

    var supplies = inventory.Supplies
      .Select(MapSupply)
      .ToList();

    var totalInvested = supplies.Sum(supply => supply.TotalPrice ?? 0m);

    var response = inventory.Adapt<InventoryResponse>();
    response.Supplies = supplies;
    response.TotalInvested = totalInvested > 0 ? totalInvested : inventory.TotalInvested;

    return await ResponseWrapper<InventoryResponse>.SuccessAsync(response);
  }

  internal static SupplyResponse MapSupply(Domain.Entities.Supply supply)
  {
    var response = supply.Adapt<SupplyResponse>();
    response.TotalPrice = supply.Quantity.HasValue && supply.Price.HasValue
      ? supply.Quantity.Value * supply.Price.Value
      : null;
    return response;
  }

  internal static FinalProductResponse MapFinalProduct(Domain.Entities.FinalProduct finalProduct)
  {
    return finalProduct.Adapt<FinalProductResponse>();
  }
}