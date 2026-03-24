using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Queries;

public record GetFinalProductByIdQuery(string Id) : IRequest<ResponseWrapper<FinalProductResponse>>;

public class GetFinalProductByIdQueryHandler(IInventoryService inventoryService) : IRequestHandler<GetFinalProductByIdQuery, ResponseWrapper<FinalProductResponse>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<FinalProductResponse>> Handle(GetFinalProductByIdQuery request, CancellationToken cancellationToken)
  {
    var finalProduct = await _inventoryService.GetFinalProductByIdAsync(request.Id);

    if (finalProduct is null)
      return await ResponseWrapper<FinalProductResponse>.FailAsync("Produto final nao encontrado.");

    return await ResponseWrapper<FinalProductResponse>.SuccessAsync(GetInventoryQueryHandler.MapFinalProduct(finalProduct));
  }
}