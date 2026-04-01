using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Commands;

public class RemoveRecipeItemCommand(string recipeItemId) : IRequest<IResponseWrapper>, IValidateMe
{
  public string RecipeItemId { get; set; } = recipeItemId;
}

public class RemoveRecipeItemCommandHandler(IInventoryService inventoryService) : IRequestHandler<RemoveRecipeItemCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(RemoveRecipeItemCommand request, CancellationToken cancellationToken)
  {
    var existing = await _inventoryService.GetRecipeItemByIdAsync(request.RecipeItemId);
    if (existing is null)
      return await ResponseWrapper.FailAsync("Item de receita nao encontrado.");

    var removedId = await _inventoryService.RemoveRecipeItemAsync(request.RecipeItemId);

    return await ResponseWrapper.SuccessAsync($"Item de receita removido. Id: {removedId}");
  }
}
