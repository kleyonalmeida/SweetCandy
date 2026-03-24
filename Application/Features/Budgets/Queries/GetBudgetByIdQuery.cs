using Application.Features.Budgets.DTOs;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Budgets.Queries;

public record GetBudgetByIdQuery(string Id) : IRequest<ResponseWrapper<BudgetResponse>>;

public class GetBudgetByIdQueryHandler(IBudgetService budgetService) : IRequestHandler<GetBudgetByIdQuery, ResponseWrapper<BudgetResponse>>
{
  private readonly IBudgetService _budgetService = budgetService;

  public async Task<ResponseWrapper<BudgetResponse>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
  {
    var budget = await _budgetService.GetByIdAsync(request.Id);

    if (budget is null)
      return await ResponseWrapper<BudgetResponse>.FailAsync("Orcamento nao encontrado.");

    return await ResponseWrapper<BudgetResponse>.SuccessAsync(MapBudget(budget));
  }

  internal static BudgetResponse MapBudget(Budget budget)
  {
    var items = budget.Items
      .Select(item =>
      {
        var response = item.Adapt<BudgetItemResponse>();
        response.TotalPrice ??= item.UnitPrice.HasValue ? item.UnitPrice.Value * item.Quantity : null;
        return response;
      })
      .ToList();

    var totalFromItems = items.Count > 0
      ? items.Sum(item => item.TotalPrice ?? 0m)
      : (decimal?)null;

    var response = budget.Adapt<BudgetResponse>();
    response.Items = items;
    response.FinalTotalValue = totalFromItems ?? budget.FinalTotalValue;
    return response;
  }
}


