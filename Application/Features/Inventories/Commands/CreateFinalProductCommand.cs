using Application.Features.Inventories.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Inventories.Commands;

public class CreateFinalProductCommand(CreateFinalProductRequest finalProduct) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateFinalProductRequest FinalProduct { get; set; } = finalProduct;
}

public class CreateFinalProductCommandHandler(IInventoryService inventoryService) : IRequestHandler<CreateFinalProductCommand, IResponseWrapper>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(CreateFinalProductCommand request, CancellationToken cancellationToken)
  {
    var finalProduct = request.FinalProduct.Adapt<FinalProduct>();

    var createdFinalProductId = await _inventoryService.CreateFinalProductAsync(finalProduct);

    return await ResponseWrapper.SuccessAsync($"Produto final criado com sucesso. Id: {createdFinalProductId}");
  }

}