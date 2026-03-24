using Application.Features.Inventories.DTOs;
using Application.Common.Mappings;
using Application.Pipelines;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Commands;

public record UpdateSupplyCommand(string Id, UpdateSupplyRequest Supply) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateSupplyCommandHandler(IInventoryService inventoryService) : IRequestHandler<UpdateSupplyCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(UpdateSupplyCommand request, CancellationToken cancellationToken)
  {
    var supply = await _inventoryService.GetSupplyByIdAsync(request.Id);

    if (supply is null)
      return await ResponseWrapper.FailAsync("Insumo nao encontrado.");

    request.Supply.Adapt(supply, MapsterSettings.IgnoreNullValues);

    supply.UpdatedAt = DateTime.UtcNow;

    var serviceMessage = await _inventoryService.UpdateSupplyAsync(supply);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Insumo atualizado com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }

}