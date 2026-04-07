using Application.Features.Orders.DTOs;
using Application.Common.Mappings;
using Application.Features.Receipts;
using Application.Features.StockMovements;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.Features.Orders.Commands;

public record UpdateOrderCommand(string Id, UpdateOrderRequest UpdateOrder) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateOrderCommandHandler(
  IOrdersService ordersService,
  IStockMovementService stockMovementService,
  IReceiptsService receiptsService) : IRequestHandler<UpdateOrderCommand, IResponseWrapper>
{
  private readonly IOrdersService _ordersService = ordersService;
  private readonly IStockMovementService _stockMovementService = stockMovementService;
  private readonly IReceiptsService _receiptsService = receiptsService;

  public async Task<IResponseWrapper> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
  {
    var order = await _ordersService.GetByIdAsync(request.Id);

    if (order is null)
      return await ResponseWrapper.FailAsync("Pedido nao encontrado.");

    var previousStatus = order.Status;

    ApplyUpdates(order, request.UpdateOrder);

    var serviceMessage = await _ordersService.UpdateAsync(order);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Pedido atualizado com sucesso."
      : serviceMessage;

    if (previousStatus != StatusOrder.Confirmada && order.Status == StatusOrder.Confirmada)
    {
      var stockErrors = new List<string>();
      foreach (var item in order.Items)
      {
        var movement = new StockMovement
        {
          SupplyId = item.FinalProductId,
          Quantity = item.Quantity,
          Type = MovementType.Saida,
          OrderId = order.Id,
          Notes = $"Baixa automatica de estoque para o pedido {order.Id}"
        };
        var result = await _stockMovementService.CreateAsync(movement);
        if (!Guid.TryParse(result, out _))
          stockErrors.Add(result);
      }
      if (stockErrors.Count > 0)
        return await ResponseWrapper.FailAsync(
          $"Pedido atualizado, mas houve erro(s) na baixa de estoque: {string.Join("; ", stockErrors)}");
    }

    if (previousStatus != StatusOrder.Concluida && order.Status == StatusOrder.Concluida)
    {
      var receipt = new Receipt
      {
        Date = DateTime.UtcNow,
        Amount = order.TotalValue ?? 0m,
        Description = $"Recebimento automatico do pedido {order.Id}",
        OrderId = order.Id,
        CustomerId = order.CustomerId,
        PaymentMethod = FormaPagamento.Dinheiro
      };
      await _receiptsService.CreateAsync(receipt);
    }

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