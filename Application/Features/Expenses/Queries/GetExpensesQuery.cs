using Application.Features.Expenses.DTOs;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Expenses.Queries;

public record GetExpensesQuery(int Page = 1, int PageSize = 20, DateTime? From = null, DateTime? To = null)
  : IRequest<ResponseWrapper<List<ExpenseResponse>>>;

public class GetExpensesQueryHandler(IExpenseService expenseService) : IRequestHandler<GetExpensesQuery, ResponseWrapper<List<ExpenseResponse>>>
{
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<ResponseWrapper<List<ExpenseResponse>>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
  {
    var all = await _expenseService.GetAllAsync();
    IEnumerable<Expense> filtered = all;

    if (request.From is { } from)
      filtered = filtered.Where(e => e.Date >= from.Date);
    if (request.To is { } to)
    {
      var end = to.Date.AddDays(1).AddTicks(-1);
      filtered = filtered.Where(e => e.Date <= end);
    }

    var response = filtered
      .OrderByDescending(e => e.Date)
      .Skip((request.Page - 1) * request.PageSize)
      .Take(request.PageSize)
      .Select(e => e.Adapt<ExpenseResponse>())
      .ToList();

    return await ResponseWrapper<List<ExpenseResponse>>.SuccessAsync(response);
  }
}
