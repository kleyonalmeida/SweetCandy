using Application.Features.Orders.DTOs;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Orders.Queries;

public record GetOrderByIdQuery(string Id) : IRequest<ResponseWrapper<OrderResponse>>;

public class GetOrderByIdQueryHandler(IOrdersService ordersService) : IRequestHandler<GetOrderByIdQuery, ResponseWrapper<OrderResponse>>
{
  private readonly IOrdersService _ordersService = ordersService;

  public async Task<ResponseWrapper<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
  {
    var order = await _ordersService.GetByIdAsync(request.Id);

    if (order is null)
      return await ResponseWrapper<OrderResponse>.FailAsync("Pedido nao encontrado.");

    return await ResponseWrapper<OrderResponse>.SuccessAsync(MapOrder(order));
  }

  internal static OrderResponse MapOrder(Order order)
  {
    var items = order.Items
      .Select(item => item.Adapt<OrderItemResponse>())
      .ToList();

    var totalFromItems = items.Count > 0
      ? items.Sum(item => item.TotalPrice)
      : (decimal?)null;

    var response = order.Adapt<OrderResponse>();
    response.Items = items;
    response.TotalValue = totalFromItems ?? order.TotalValue;
    return response;
  }
}