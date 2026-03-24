using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Commands;

public record DeleteSupplyCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

public class DeleteSupplyCommandHandler(IInventoryService inventoryService) : IRequestHandler<DeleteSupplyCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(DeleteSupplyCommand request, CancellationToken cancellationToken)
  {
    var supply = await _inventoryService.GetSupplyByIdAsync(request.Id);

    if (supply is null)
      return await ResponseWrapper.FailAsync("Insumo nao encontrado.");

    var serviceMessage = await _inventoryService.DeleteSupplyAsync(supply);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Insumo removido com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}