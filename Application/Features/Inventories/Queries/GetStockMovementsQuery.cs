using Application.Features.Inventories.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Inventories.Queries;

public record GetStockMovementsQuery(string? SupplyId = null, int Page = 1, int PageSize = 50)
  : IRequest<ResponseWrapper<List<StockMovementResponse>>>;

public class GetStockMovementsQueryHandler(IInventoryService inventoryService)
  : IRequestHandler<GetStockMovementsQuery, ResponseWrapper<List<StockMovementResponse>>>
{
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<ResponseWrapper<List<StockMovementResponse>>> Handle(GetStockMovementsQuery request, CancellationToken cancellationToken)
  {
    var movements = await _inventoryService.GetStockMovementsAsync(request.SupplyId);
    var response = movements
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(m => new StockMovementResponse
      {
        Id = m.Id,
        Date = m.Date,
        SupplyId = m.SupplyId,
        SupplyName = m.Supply?.Name,
        Quantity = m.Quantity,
        Type = m.Type,
        Notes = m.Notes
      })
      .ToList();

    return await ResponseWrapper<List<StockMovementResponse>>.SuccessAsync(response);
  }
}
