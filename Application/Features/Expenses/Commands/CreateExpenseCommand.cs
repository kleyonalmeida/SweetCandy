using Application.Features.Expenses.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Expenses.Commands;

public class CreateExpenseCommand(CreateExpenseRequest createExpense) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateExpenseRequest CreateExpense { get; set; } = createExpense;
}

public class CreateExpenseCommandHandler(IExpenseService expenseService) : IRequestHandler<CreateExpenseCommand, IResponseWrapper>
{
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<IResponseWrapper> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
  {
    var expense = request.CreateExpense.Adapt<Expense>();

    var createdExpenseId = await _expenseService.CreateAsync(expense);

    return await ResponseWrapper.SuccessAsync($"Despesa criada com sucesso. Id: {createdExpenseId}");
  }
}
