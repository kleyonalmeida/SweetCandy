using Application.Features.StockMovements.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.StockMovements.Queries;

public record GetStockMovementsByOrderIdQuery(string OrderId) : IRequest<ResponseWrapper<List<StockMovementResponse>>>;

public class GetStockMovementsByOrderIdQueryHandler(IStockMovementService stockMovementService) : IRequestHandler<GetStockMovementsByOrderIdQuery, ResponseWrapper<List<StockMovementResponse>>>
{
  private readonly IStockMovementService _stockMovementService = stockMovementService;

  public async Task<ResponseWrapper<List<StockMovementResponse>>> Handle(GetStockMovementsByOrderIdQuery request, CancellationToken cancellationToken)
  {
    var movements = await _stockMovementService.GetByOrderIdAsync(request.OrderId);

    var projected = movements
      .Select(movement => movement.Adapt<StockMovementResponse>())
      .ToList();

    return await ResponseWrapper<List<StockMovementResponse>>.SuccessAsync(projected);
  }
}
