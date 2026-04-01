using Application.Features.Inventories.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Commands;

public class AddRecipeItemCommand(string finalProductId, AddRecipeItemRequest request) : IRequest<IResponseWrapper>, IValidateMe
{
  public string FinalProductId { get; set; } = finalProductId;
  public AddRecipeItemRequest Request { get; set; } = request;
}

public class AddRecipeItemCommandHandler(IInventoryService inventoryService) : IRequestHandler<AddRecipeItemCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(AddRecipeItemCommand request, CancellationToken cancellationToken)
  {
    var finalProduct = await _inventoryService.GetFinalProductByIdAsync(request.FinalProductId);
    if (finalProduct is null)
      return await ResponseWrapper.FailAsync("Produto final nao encontrado.");

    if (string.IsNullOrWhiteSpace(request.Request.SupplyId))
      return await ResponseWrapper.FailAsync("SupplyId eh requerido.");

    var supply = await _inventoryService.GetSupplyByIdAsync(request.Request.SupplyId!);
    if (supply is null)
      return await ResponseWrapper.FailAsync("Insumo nao encontrado.");

    var item = new RecipeItem
    {
      FinalProductId = request.FinalProductId,
      SupplyId = request.Request.SupplyId,
      Quantity = request.Request.Quantity,
      Unit = request.Request.Unit
    };

    var createdId = await _inventoryService.AddRecipeItemAsync(item);

    return await ResponseWrapper.SuccessAsync($"Item de receita adicionado. Id: {createdId}");
  }
}
