using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Queries;

public class GetRecipeQuery(string finalProductId) : IRequest<ResponseWrapper<List<RecipeItemResponse>>>
{
  public string FinalProductId { get; set; } = finalProductId;
}

public class GetRecipeQueryHandler(IInventoryService inventoryService) : IRequestHandler<GetRecipeQuery, ResponseWrapper<List<RecipeItemResponse>>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<List<RecipeItemResponse>>> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
  {
    var items = await _inventoryService.GetRecipeAsync(request.FinalProductId);
    var result = new List<RecipeItemResponse>();

    foreach (var it in items)
    {
      var supply = it.SupplyId is null ? null : await _inventoryService.GetSupplyByIdAsync(it.SupplyId);
      result.Add(new RecipeItemResponse
      {
        Id = it.Id,
        FinalProductId = it.FinalProductId,
        SupplyId = it.SupplyId,
        SupplyName = supply?.Name,
        Quantity = it.Quantity,
        Unit = it.Unit
      });
    }

    return ResponseWrapper<List<RecipeItemResponse>>.Success(result);
  }
}
