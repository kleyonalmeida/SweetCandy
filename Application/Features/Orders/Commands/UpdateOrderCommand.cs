using Application.Features.Orders.DTOs;
using Application.Common.Mappings;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Orders.Commands;

public record UpdateOrderCommand(string Id, UpdateOrderRequest UpdateOrder) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateOrderCommandHandler(IOrdersService ordersService) : IRequestHandler<UpdateOrderCommand, IResponseWrapper>
{
  private readonly IOrdersService _ordersService = ordersService;

  public async Task<IResponseWrapper> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
  {
    var order = await _ordersService.GetByIdAsync(request.Id);

    if (order is null)
      return await ResponseWrapper.FailAsync("Pedido nao encontrado.");

    ApplyUpdates(order, request.UpdateOrder);

    var serviceMessage = await _ordersService.UpdateAsync(order);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Pedido atualizado com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }

  private static void ApplyUpdates(Order order, UpdateOrderRequest updateOrder)
  {
    updateOrder.Adapt(order, MapsterSettings.IgnoreNullValues);

    if (updateOrder.Items.Count > 0)
      order.Items = MapItems(updateOrder.Items);

    order.TotalValue = ResolveTotalValue(updateOrder.TotalValue, order.Items.ToList(), order.TotalValue);
    order.UpdatedAt = DateTime.UtcNow;
  }

  private static decimal? ResolveTotalValue(decimal? informedTotal, List<OrderItem> items, decimal? currentTotal)
  {
    if (informedTotal.HasValue)
      return informedTotal.Value;

    if (items.Count > 0)
      return items.Sum(item => item.TotalPrice);

    return currentTotal;
  }

  private static List<OrderItem> MapItems(List<CreateOrderItemRequest> items)
  {
    return items
      .Where(item => !string.IsNullOrWhiteSpace(item.FinalProductName) || !string.IsNullOrWhiteSpace(item.FinalProductId))
      .Select(item =>
      {
        var orderItem = item.Adapt<OrderItem>();
        orderItem.UnitPrice = item.UnitPrice ?? 0m;
        orderItem.TotalPrice = (item.UnitPrice ?? 0m) * item.Quantity;
        return orderItem;
      })
      .ToList();
  }

}