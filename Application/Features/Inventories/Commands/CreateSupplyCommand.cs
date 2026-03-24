using Application.Features.Inventories.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Commands;

public class CreateSupplyCommand(CreateSupplyRequest supply) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateSupplyRequest Supply { get; set; } = supply;
}

public class CreateSupplyCommandHandler(IInventoryService inventoryService) : IRequestHandler<CreateSupplyCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
  {
    var supply = request.Supply.Adapt<Supply>();

    var createdSupplyId = await _inventoryService.CreateSupplyAsync(supply);

    return await ResponseWrapper.SuccessAsync($"Insumo criado com sucesso. Id: {createdSupplyId}");
  }


}