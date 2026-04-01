using Application.Features.StockMovements.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.StockMovements.Queries;

public record GetStockMovementsQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<StockMovementResponse>>>;

public class GetStockMovementsQueryHandler(IStockMovementService stockMovementService) : IRequestHandler<GetStockMovementsQuery, ResponseWrapper<List<StockMovementResponse>>>
{
  private readonly IStockMovementService _stockMovementService = stockMovementService;

  public async Task<ResponseWrapper<List<StockMovementResponse>>> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken)
  {
    var movements = await _stockMovementService.GetAllAsync();

    var projected = movements
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(movement => movement.Adapt<StockMovementResponse>())
      .ToList();

    return await ResponseWrapper<List<StockMovementResponse>>.SuccessAsync(projected);
  }
}
