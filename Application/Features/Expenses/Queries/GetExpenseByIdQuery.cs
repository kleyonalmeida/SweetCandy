using Application.Features.Expenses.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Expenses.Queries;

public record GetExpenseByIdQuery(string Id) : IRequest<ResponseWrapper<ExpenseResponse>>;

public class GetExpenseByIdQueryHandler(IExpenseService expenseService) : IRequestHandler<GetExpenseByIdQuery, ResponseWrapper<ExpenseResponse>>
{
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<ResponseWrapper<ExpenseResponse>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
  {
    var expense = await _expenseService.GetByIdAsync(request.Id);

    if (expense is null)
      return await ResponseWrapper<ExpenseResponse>.FailAsync("Despesa nao encontrada.");

    return await ResponseWrapper<ExpenseResponse>.SuccessAsync(expense.Adapt<ExpenseResponse>());
  }
}
