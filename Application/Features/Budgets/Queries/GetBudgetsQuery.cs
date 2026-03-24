using Application.Features.Budgets.DTOs;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Budgets.Queries;

public record GetBudgetsQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<BudgetResponse>>>;

public class GetBudgetsQueryHandler(IBudgetService budgetService) : IRequestHandler<GetBudgetsQuery, ResponseWrapper<List<BudgetResponse>>>
{
  private readonly IBudgetService _budgetService = budgetService;

  public async Task<ResponseWrapper<List<BudgetResponse>>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
  {
    var budgets = await _budgetService.GetAllAsync();

    var projectedBudgets = budgets
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(GetBudgetByIdQueryHandler.MapBudget)
      .ToList();

    return await ResponseWrapper<List<BudgetResponse>>.SuccessAsync(projectedBudgets);
  }
}

