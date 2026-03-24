using Application.Wrappers;
using MediatR;

namespace Application.Features.Budgets.Commands;

public class DeleteBudgetCommandHandler(IBudgetService budgetService) : IRequestHandler<DeleteBudgetCommand, IResponseWrapper>
{
  private readonly IBudgetService _budgetService = budgetService;

  public async Task<IResponseWrapper> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
  {
    var budget = await _budgetService.GetByIdAsync(request.Id);

    if (budget is null)
      return await ResponseWrapper.FailAsync("Orcamento nao encontrado.");

    var serviceMessage = await _budgetService.DeleteAsync(budget);
    var successMessage = string.IsNullOrWhiteSpace(serviceMessage)
      ? "Orcamento removido com sucesso."
      : serviceMessage;

    return await ResponseWrapper.SuccessAsync(successMessage);
  }
}