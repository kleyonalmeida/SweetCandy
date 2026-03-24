using Application.Features.Budgets.DTOs;
using Mapster;
using Application.Wrappers;
using MediatR;
using Application.Pipelines;
using Domain.Entities;
using System.Linq;

namespace Application.Features.Budgets.Commands;

public class CreateBudgetCommand(CreateBudgetRequest createBudget) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateBudgetRequest CreateBudget { get; set; } = createBudget;
}
public class CreateBudgetCommandHandler(IBudgetService budgetService) : IRequestHandler<CreateBudgetCommand, IResponseWrapper>
{
  private readonly IBudgetService _budgetService = budgetService;

  public async Task<IResponseWrapper> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
  {
    var items = MapItems(request.CreateBudget.Items);

    var budget = request.CreateBudget.Adapt<Budget>();
    budget.Items = items;
    budget.FinalTotalValue = ResolveTotalValue(
      request.CreateBudget.FinalTotalValue,
      request.CreateBudget.FinalProductQuantity,
      request.CreateBudget.FinalUnitPrice,
      items);

    var createdBudgetId = await _budgetService.CreateAsync(budget);

    return await ResponseWrapper.SuccessAsync($"Orcamento criado com sucesso. Id: {createdBudgetId}");
  }

  private static decimal? ResolveTotalValue(decimal? informedTotal, decimal? quantity, decimal? unitPrice)
  {
    if (informedTotal.HasValue)
      return informedTotal.Value;

    if (quantity.HasValue && unitPrice.HasValue)
      return quantity.Value * unitPrice.Value;

    return null;
  }

  private static decimal? ResolveTotalValue(decimal? informedTotal, decimal? quantity, decimal? unitPrice, List<BudgetItem> items)
  {
    if (informedTotal.HasValue)
      return informedTotal.Value;

    if (items.Count > 0)
      return items.Sum(item => item.TotalPrice ?? 0m);

    if (quantity.HasValue && unitPrice.HasValue)
      return quantity.Value * unitPrice.Value;

    return null;
  }

  private static List<BudgetItem> MapItems(List<CreateBudgetItemRequest> items)
  {
    return items
      .Where(item => !string.IsNullOrWhiteSpace(item.FinalProductName) || !string.IsNullOrWhiteSpace(item.FinalProductId))
      .Select(item =>
      {
        var budgetItem = item.Adapt<BudgetItem>();
        budgetItem.TotalPrice = item.UnitPrice.HasValue ? item.UnitPrice.Value * item.Quantity : null;
        return budgetItem;
      })
      .ToList();
  }
}