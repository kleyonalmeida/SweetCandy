using Application.Features.Budgets.DTOs;
using Application.Common.Mappings;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;
using System.Linq;

namespace Application.Features.Budgets.Commands;

public record UpdateBudgetCommand(string Id, UpdateBudgetRequest UpdateBudget) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateBudgetCommandHandler(IBudgetService budgetService) : IRequestHandler<UpdateBudgetCommand, IResponseWrapper>
{
  private readonly IBudgetService _budgetService = budgetService;

  public async Task<IResponseWrapper> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
  {
    var budget = await _budgetService.GetByIdAsync(request.Id);

    if (budget is null)
      return await ResponseWrapper.FailAsync("Orcamento nao encontrado.");

    ApplyUpdates(budget, request);

    var serviceMessage = await _budgetService.UpdateAsync(budget);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Orcamento atualizado com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }

  private static void ApplyUpdates(Budget budget, UpdateBudgetCommand request)
  {
    request.UpdateBudget.Adapt(budget, MapsterSettings.IgnoreNullValues);

    if (request.UpdateBudget.Items.Count > 0)
      budget.SetItems(MapItems(request.UpdateBudget.Items));

    budget.FinalTotalValue = ResolveTotalValue(request.UpdateBudget, budget);
    budget.MarkUpdated();
  }

  private static decimal? ResolveTotalValue(UpdateBudgetRequest updateBudget, Budget budget)
  {
    if (updateBudget.FinalTotalValue.HasValue)
      return updateBudget.FinalTotalValue.Value;

    if (budget.Items.Count > 0)
      return budget.Items.Sum(item => item.TotalPrice ?? 0m);

    if (budget.FinalProductQuantity.HasValue && budget.FinalUnitPrice.HasValue)
      return budget.FinalProductQuantity.Value * budget.FinalUnitPrice.Value;

    return budget.FinalTotalValue;
  }

  private static List<BudgetItem> MapItems(List<CreateBudgetItemRequest> items)
  {
    return items
      .Where(item => !string.IsNullOrWhiteSpace(item.FinalProductName) || !string.IsNullOrWhiteSpace(item.FinalProductId))
      .Select(item =>
      {
        var budgetItem = item.Adapt<BudgetItem>();
        return budgetItem;
      })
      .ToList();
  }

}