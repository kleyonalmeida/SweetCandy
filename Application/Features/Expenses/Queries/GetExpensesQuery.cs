using Application.Features.Expenses.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Expenses.Queries;

public record GetExpensesQuery(int Page = 1, int PageSize = 20) : IRequest<ResponseWrapper<List<ExpenseResponse>>>;

public class GetExpensesQueryHandler(IExpenseService expenseService) : IRequestHandler<GetExpensesQuery, ResponseWrapper<List<ExpenseResponse>>>
{
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<ResponseWrapper<List<ExpenseResponse>>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
  {
    var expenses = await _expenseService.GetAllAsync();

    var projected = expenses
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(expense => expense.Adapt<ExpenseResponse>())
      .ToList();

    return await ResponseWrapper<List<ExpenseResponse>>.SuccessAsync(projected);
  }
}
