using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Queries;

public record GetSupplyByIdQuery(string Id) : IRequest<ResponseWrapper<SupplyResponse>>;

public class GetSupplyByIdQueryHandler(IInventoryService inventoryService) : IRequestHandler<GetSupplyByIdQuery, ResponseWrapper<SupplyResponse>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<SupplyResponse>> Handle(GetSupplyByIdQuery request, CancellationToken cancellationToken)
  {
    var supply = await _inventoryService.GetSupplyByIdAsync(request.Id);

    if (supply is null)
      return await ResponseWrapper<SupplyResponse>.FailAsync("Insumo nao encontrado.");

    return await ResponseWrapper<SupplyResponse>.SuccessAsync(GetInventoryQueryHandler.MapSupply(supply));
  }
}