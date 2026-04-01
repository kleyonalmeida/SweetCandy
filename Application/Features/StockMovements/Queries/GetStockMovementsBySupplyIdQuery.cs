using Application.Features.StockMovements.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.StockMovements.Queries;

public record GetStockMovementsBySupplyIdQuery(string SupplyId) : IRequest<ResponseWrapper<List<StockMovementResponse>>>;

public class GetStockMovementsBySupplyIdQueryHandler(IStockMovementService stockMovementService) : IRequestHandler<GetStockMovementsBySupplyIdQuery, ResponseWrapper<List<StockMovementResponse>>>
{
  private readonly IStockMovementService _stockMovementService = stockMovementService;

  public async Task<ResponseWrapper<List<StockMovementResponse>>> Handle(GetStockMovementsBySupplyIdQuery request, CancellationToken cancellationToken)
  {
    var movements = await _stockMovementService.GetBySupplyIdAsync(request.SupplyId);

    var projected = movements
      .Select(movement => movement.Adapt<StockMovementResponse>())
      .ToList();

    return await ResponseWrapper<List<StockMovementResponse>>.SuccessAsync(projected);
  }
}
