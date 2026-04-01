using Application.Features.Inventories;
using Application.Features.Orders;
using Application.Features.StockMovements.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.StockMovements.Commands;

public class CreateStockMovementCommand(CreateStockMovementRequest createStockMovement) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateStockMovementRequest CreateStockMovement { get; set; } = createStockMovement;
}

public class CreateStockMovementCommandHandler(
  IStockMovementService stockMovementService,
  IInventoryService inventoryService,
  IOrdersService ordersService) : IRequestHandler<CreateStockMovementCommand, IResponseWrapper>
{
  private readonly IStockMovementService _stockMovementService = stockMovementService;
  private readonly IInventoryService _inventoryService = inventoryService;
  private readonly IOrdersService _ordersService = ordersService;

  public async Task<IResponseWrapper> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken)
  {
    if (!string.IsNullOrWhiteSpace(request.CreateStockMovement.SupplyId))
    {
      var supply = await _inventoryService.GetSupplyByIdAsync(request.CreateStockMovement.SupplyId);
      if (supply is null)
        return await ResponseWrapper.FailAsync("Insumo nao encontrado.");
    }

    if (!string.IsNullOrWhiteSpace(request.CreateStockMovement.OrderId))
    {
      var order = await _ordersService.GetByIdAsync(request.CreateStockMovement.OrderId);
      if (order is null)
        return await ResponseWrapper.FailAsync("Pedido nao encontrado.");
    }

    var movement = request.CreateStockMovement.Adapt<StockMovement>();

    var createdId = await _stockMovementService.CreateAsync(movement);

    return await ResponseWrapper.SuccessAsync($"Movimentacao de estoque criada com sucesso. Id: {createdId}");
  }
}
