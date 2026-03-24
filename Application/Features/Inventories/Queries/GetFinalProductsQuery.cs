using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Queries;

public record GetFinalProductsQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<FinalProductResponse>>>;

public class GetFinalProductsQueryHandler(IInventoryService inventoryService) : IRequestHandler<GetFinalProductsQuery, ResponseWrapper<List<FinalProductResponse>>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<List<FinalProductResponse>>> Handle(GetFinalProductsQuery request, CancellationToken cancellationToken)
  {
    var page = request.Page < 1 ? 1 : request.Page;
    var pageSize = request.PageSize < 1 ? 20 : request.PageSize;

    var finalProducts = await _inventoryService.GetFinalProductsAsync();

    var projectedFinalProducts = finalProducts
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(GetFinalProductsQueryHandler.MapFinalProduct)
      .ToList();

    return await ResponseWrapper<List<FinalProductResponse>>.SuccessAsync(projectedFinalProducts);
  }

  internal static FinalProductResponse MapFinalProduct(Domain.Entities.FinalProduct finalProduct)
  {
    return finalProduct.Adapt<FinalProductResponse>();
  }
}