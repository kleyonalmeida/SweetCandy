using Application.Features.Inventories.DTOs;
using Application.Features.StockMovements;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Inventories.Commands;

public class AdjustSupplyStockCommand(string supplyId, AdjustSupplyStockRequest request) : IRequest<ResponseWrapper<AdjustSupplyStockResponse>>
{
  public string SupplyId { get; set; } = supplyId;
  public AdjustSupplyStockRequest Request { get; set; } = request;
}

public class AdjustSupplyStockCommandHandler(
  IInventoryService inventoryService,
  IStockMovementService stockMovementService)
  : IRequestHandler<AdjustSupplyStockCommand, ResponseWrapper<AdjustSupplyStockResponse>>
{
  private readonly IInventoryService _inventoryService = inventoryService;
  private readonly IStockMovementService _stockMovementService = stockMovementService;

  public async Task<ResponseWrapper<AdjustSupplyStockResponse>> Handle(AdjustSupplyStockCommand request, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.SupplyId))
      return ResponseWrapper<AdjustSupplyStockResponse>.Fail("Insumo invalido.");
    if (request.Request.Quantity <= 0)
      return ResponseWrapper<AdjustSupplyStockResponse>.Fail("Quantidade deve ser maior que zero.");

    var supply = await _inventoryService.GetSupplyByIdAsync(request.SupplyId);
    if (supply is null)
      return ResponseWrapper<AdjustSupplyStockResponse>.Fail("Insumo nao encontrado.");

    var previous = supply.Quantity ?? 0m;
    var delta = request.Request.Add ? request.Request.Quantity : -request.Request.Quantity;
    var newQty = previous + delta;

    if (newQty < 0)
      return ResponseWrapper<AdjustSupplyStockResponse>.Fail("Estoque insuficiente para esta saida.");

    supply.Quantity = newQty;
    await _inventoryService.UpdateSupplyAsync(supply);

    var movement = new StockMovement
    {
      SupplyId = supply.Id,
      Quantity = request.Request.Quantity,
      Type = request.Request.Add ? MovementType.Entrada : MovementType.Saida,
      Notes = request.Request.Notes
    };
    var movementId = await _stockMovementService.CreateAsync(movement);

    var result = new AdjustSupplyStockResponse
    {
      PreviousQuantity = previous,
      NewQuantity = newQty,
      MovementId = movementId
    };

    return await ResponseWrapper<AdjustSupplyStockResponse>.SuccessAsync(result);
  }
}
