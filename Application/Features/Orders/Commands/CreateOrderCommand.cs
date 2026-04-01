using Application.Features.Customers;
using Application.Features.Inventories;
using Application.Features.Orders.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Orders.Commands;

public class CreateOrderCommand(CreateOrderRequest createOrder) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateOrderRequest CreateOrder { get; set; } = createOrder;
}

public class CreateOrderCommandHandler(
  IOrdersService ordersService,
  ICustomerService customerService,
  IInventoryService inventoryService) : IRequestHandler<CreateOrderCommand, IResponseWrapper>
{
  private readonly IOrdersService _ordersService = ordersService;
  private readonly ICustomerService _customerService = customerService;
  private readonly IInventoryService _inventoryService = inventoryService;

  public async Task<IResponseWrapper> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
  {
    if (!string.IsNullOrWhiteSpace(request.CreateOrder.CustomerId))
    {
      var customer = await _customerService.GetByIdAsync(request.CreateOrder.CustomerId);
      if (customer is null)
        return await ResponseWrapper.FailAsync("Cliente nao encontrado.");
    }

    foreach (var item in request.CreateOrder.Items)
    {
      if (!string.IsNullOrWhiteSpace(item.FinalProductId))
      {
        var product = await _inventoryService.GetFinalProductByIdAsync(item.FinalProductId);
        if (product is null)
          return await ResponseWrapper.FailAsync($"Produto final '{item.FinalProductId}' nao encontrado.");
      }
    }

    var items = MapItems(request.CreateOrder.Items);

    var order = request.CreateOrder.Adapt<Order>();
    order.Items = items;
    order.TotalValue = ResolveTotalValue(request.CreateOrder.TotalValue, items);

    var createdOrderId = await _ordersService.CreateAsync(order);

    return await ResponseWrapper.SuccessAsync($"Pedido criado com sucesso. Id: {createdOrderId}");
  }


  private static decimal? ResolveTotalValue(decimal? informedTotal, List<OrderItem> items)
  {
    if (informedTotal.HasValue)
      return informedTotal.Value;

    if (items.Count > 0)
      return items.Sum(item => item.TotalPrice);

    return null;
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