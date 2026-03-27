using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features.Expenses.Commands;

public class DeleteExpenseCommand(string id) : IRequest<IResponseWrapper>, IValidateMe
{
  public string Id { get; set; } = id;
}

public class DeleteExpenseCommandHandler(IExpenseService expenseService) : IRequestHandler<DeleteExpenseCommand, IResponseWrapper>
{
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<IResponseWrapper> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
  {
    var expense = new Expense { Id = request.Id };
    var result = await _expenseService.DeleteAsync(expense);
    if (!string.IsNullOrWhiteSpace(result))
      return await ResponseWrapper.FailAsync(result);
    return await ResponseWrapper.SuccessAsync("Despesa excluida com sucesso.");
  }
}
