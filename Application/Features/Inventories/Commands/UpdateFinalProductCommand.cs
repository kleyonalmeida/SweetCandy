using Application.Features.Inventories.DTOs;
using Application.Common.Mappings;
using Application.Pipelines;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Commands;

public record UpdateFinalProductCommand(string Id, UpdateFinalProductRequest FinalProduct) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateFinalProductCommandHandler(IInventoryService inventoryService) : IRequestHandler<UpdateFinalProductCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(UpdateFinalProductCommand request, CancellationToken cancellationToken)
  {
    var finalProduct = await _inventoryService.GetFinalProductByIdAsync(request.Id);

    if (finalProduct is null)
      return await ResponseWrapper.FailAsync("Produto final nao encontrado.");

    request.FinalProduct.Adapt(finalProduct, MapsterSettings.IgnoreNullValues);

    finalProduct.UpdatedAt = DateTime.UtcNow;

    var serviceMessage = await _inventoryService.UpdateFinalProductAsync(finalProduct);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Produto final atualizado com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }

}