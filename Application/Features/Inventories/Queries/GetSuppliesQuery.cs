using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Queries;

public record GetSuppliesQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<SupplyResponse>>>;

public class GetSuppliesQueryHandler(IInventoryService inventoryService) : IRequestHandler<GetSuppliesQuery, ResponseWrapper<List<SupplyResponse>>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<List<SupplyResponse>>> Handle(GetSuppliesQuery request, CancellationToken cancellationToken)
  {
    var page = request.Page < 1 ? 1 : request.Page;
    var pageSize = request.PageSize < 1 ? 20 : request.PageSize;

    var supplies = await _inventoryService.GetSuppliesAsync();

    var projectedSupplies = supplies
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(GetInventoryQueryHandler.MapSupply)
      .ToList();

    return await ResponseWrapper<List<SupplyResponse>>.SuccessAsync(projectedSupplies);
  }
}