using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Expenses.Commands;

public record DeleteExpenseCommand(string Id) : IRequest<IResponseWrapper>, IValidateMe;

public class DeleteExpenseCommandHandler(IExpenseService expenseService) : IRequestHandler<DeleteExpenseCommand, IResponseWrapper>
{
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<IResponseWrapper> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
  {
    var expense = await _expenseService.GetByIdAsync(request.Id);

    if (expense is null)
      return await ResponseWrapper.FailAsync("Despesa nao encontrada.");

    var serviceMessage = await _expenseService.DeleteAsync(expense);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Despesa removida com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}
