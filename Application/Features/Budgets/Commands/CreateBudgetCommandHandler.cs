using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features.Budgets.Commands;

public class CreateBudgetCommandHandler(IBudgetService budgetService) : IRequestHandler<CreateBudgetCommand, IResponseWrapper>
{
  private readonly IBudgetService _budgetService = budgetService;

  public async Task<IResponseWrapper> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
  {
    var budget = new Budget
    {
      ClientName = Normalize(request.CreateBudget.ClientName),
      EventDate = request.CreateBudget.EventDate,
      FinalProductName = Normalize(request.CreateBudget.FinalProductName),
      FinalProductDescription = Normalize(request.CreateBudget.FinalProductDescription),
      FinalProductQuantity = request.CreateBudget.FinalProductQuantity,
      FinalUnitPrice = request.CreateBudget.FinalUnitPrice,
      FinalTotalValue = ResolveTotalValue(
        request.CreateBudget.FinalTotalValue,
        request.CreateBudget.FinalProductQuantity,
        request.CreateBudget.FinalUnitPrice)
    };

    var createdBudgetId = await _budgetService.CreateAsync(budget);

    return await ResponseWrapper.SuccessAsync($"Orcamento criado com sucesso. Id: {createdBudgetId}");
  }

  private static string? Normalize(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
  }

  private static decimal? ResolveTotalValue(decimal? informedTotal, decimal? quantity, decimal? unitPrice)
  {
    if (informedTotal.HasValue)
      return informedTotal.Value;

    if (quantity.HasValue && unitPrice.HasValue)
      return quantity.Value * unitPrice.Value;

    return null;
  }
}