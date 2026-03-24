using Application.Features.Orders.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Orders.Queries;

public record GetOrdersQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<OrderResponse>>>;

public class GetOrdersQueryHandler(IOrdersService ordersService) : IRequestHandler<GetOrdersQuery, ResponseWrapper<List<OrderResponse>>>
{
  private readonly IOrdersService _ordersService = ordersService;

  public async Task<ResponseWrapper<List<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
  {
    var orders = await _ordersService.GetAllAsync();

    var projectedOrders = orders
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(GetOrderByIdQueryHandler.MapOrder)
      .ToList();

    return await ResponseWrapper<List<OrderResponse>>.SuccessAsync(projectedOrders);
  }
}