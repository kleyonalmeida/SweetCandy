using Application.Common.Mappings;
using Application.Features.Expenses.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Expenses.Commands;

public record UpdateExpenseCommand(string Id, UpdateExpenseRequest UpdateExpense) : IRequest<IResponseWrapper>, IValidateMe;

public class UpdateExpenseCommandHandler(IExpenseService expenseService) : IRequestHandler<UpdateExpenseCommand, IResponseWrapper>
{
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<IResponseWrapper> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
  {
    var expense = await _expenseService.GetByIdAsync(request.Id);

    if (expense is null)
      return await ResponseWrapper.FailAsync("Despesa nao encontrada.");

    request.UpdateExpense.Adapt(expense, MapsterSettings.IgnoreNullValues);

    expense.MarkUpdated();

    var serviceMessage = await _expenseService.UpdateAsync(expense);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Despesa atualizada com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}
