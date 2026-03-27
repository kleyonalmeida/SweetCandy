using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Commands;

public class AdjustSupplyStockCommand(string supplyId, AdjustSupplyStockRequest request) : IRequest<ResponseWrapper<AdjustSupplyStockResponse>>
{
  public string SupplyId { get; set; } = supplyId;
  public AdjustSupplyStockRequest Request { get; set; } = request;
}

public class AdjustSupplyStockCommandHandler(IInventoryService inventoryService)
  : IRequestHandler<AdjustSupplyStockCommand, ResponseWrapper<AdjustSupplyStockResponse>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<AdjustSupplyStockResponse>> Handle(AdjustSupplyStockCommand request, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.SupplyId))
      return ResponseWrapper<AdjustSupplyStockResponse>.Fail("Insumo invalido.");
    if (request.Request.Quantity <= 0)
      return ResponseWrapper<AdjustSupplyStockResponse>.Fail("Quantidade deve ser maior que zero.");

    var (error, result) = await _inventoryService.AdjustSupplyStockAsync(
      request.SupplyId,
      request.Request.Quantity,
      request.Request.Add,
      request.Request.Notes);

    if (error is not null)
      return ResponseWrapper<AdjustSupplyStockResponse>.Fail(error);

    return await ResponseWrapper<AdjustSupplyStockResponse>.SuccessAsync(result!);
  }
}
