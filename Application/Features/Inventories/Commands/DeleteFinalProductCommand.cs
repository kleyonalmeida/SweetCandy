using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Commands;

public record DeleteFinalProductCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

public class DeleteFinalProductCommandHandler(IInventoryService inventoryService) : IRequestHandler<DeleteFinalProductCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(DeleteFinalProductCommand request, CancellationToken cancellationToken)
  {
    var finalProduct = await _inventoryService.GetFinalProductByIdAsync(request.Id);

    if (finalProduct is null)
      return await ResponseWrapper.FailAsync("Produto final nao encontrado.");

    var serviceMessage = await _inventoryService.DeleteFinalProductAsync(finalProduct);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Produto final removido com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}