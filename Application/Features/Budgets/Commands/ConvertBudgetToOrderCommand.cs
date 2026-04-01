using Application.Features.Orders;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Budgets.Commands;

public record ConvertBudgetToOrderCommand(string BudgetId) : IRequest<IResponseWrapper>, IValidateMe;

public class ConvertBudgetToOrderCommandHandler(
  IBudgetService budgetService,
  IOrdersService ordersService) : IRequestHandler<ConvertBudgetToOrderCommand, IResponseWrapper>
{
  private readonly IBudgetService _budgetService = budgetService;
  private readonly IOrdersService _ordersService = ordersService;

  public async Task<IResponseWrapper> Handle(ConvertBudgetToOrderCommand request, CancellationToken cancellationToken)
  {
    var budget = await _budgetService.GetByIdAsync(request.BudgetId);

    if (budget is null)
      return await ResponseWrapper.FailAsync("Orcamento nao encontrado.");

    var order = new Order
    {
      Name = budget.ClientName ?? string.Empty,
      CustomerId = budget.CustomerId,
      EventDate = budget.EventDate,
      Status = StatusOrder.Pendente,
      TotalValue = budget.FinalTotalValue,
      Items = budget.Items.Select(item => new OrderItem
      {
        FinalProductId = item.FinalProductId,
        FinalProductName = item.FinalProductName,
        Quantity = item.Quantity,
        UnitPrice = item.UnitPrice ?? 0m,
        TotalPrice = item.TotalPrice ?? 0m
      }).ToList()
    };

    var createdOrderId = await _ordersService.CreateAsync(order);

    return await ResponseWrapper.SuccessAsync($"Orcamento convertido em pedido com sucesso. Pedido Id: {createdOrderId}");
  }
}
