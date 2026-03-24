using Application.Wrappers;
using Application.Features.Budgets.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Budgets.Commands;

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
    if (!string.IsNullOrWhiteSpace(request.UpdateBudget.ClientName))
      budget.ClientName = request.UpdateBudget.ClientName.Trim();

    if (request.UpdateBudget.EventDate.HasValue)
      budget.EventDate = request.UpdateBudget.EventDate.Value;

    if (!string.IsNullOrWhiteSpace(request.UpdateBudget.FinalProductName))
      budget.FinalProductName = request.UpdateBudget.FinalProductName.Trim();

    if (!string.IsNullOrWhiteSpace(request.UpdateBudget.FinalProductDescription))
      budget.FinalProductDescription = request.UpdateBudget.FinalProductDescription.Trim();

    if (request.UpdateBudget.FinalProductQuantity.HasValue)
      budget.FinalProductQuantity = request.UpdateBudget.FinalProductQuantity.Value;

    if (request.UpdateBudget.FinalUnitPrice.HasValue)
      budget.FinalUnitPrice = request.UpdateBudget.FinalUnitPrice.Value;

    budget.FinalTotalValue = ResolveTotalValue(request.UpdateBudget, budget);
    budget.UpdatedAt = DateTime.UtcNow;
  }

  private static decimal? ResolveTotalValue(UpdateBudgetRequest updateBudget, Budget budget)
  {
    if (updateBudget.FinalTotalValue.HasValue)
      return updateBudget.FinalTotalValue.Value;

    if (budget.FinalProductQuantity.HasValue && budget.FinalUnitPrice.HasValue)
      return budget.FinalProductQuantity.Value * budget.FinalUnitPrice.Value;

    return budget.FinalTotalValue;
  }
}