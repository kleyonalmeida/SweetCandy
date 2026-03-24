using Application.Features.Budgets.DTOs;
using Application.Wrappers;
using MediatR;
using Application.Pipelines;

namespace Application.Features.Budgets.Commands;

public class CreateBudgetCommand(CreateBudgetRequest createBudget) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateBudgetRequest CreateBudget { get; set; } = createBudget;
}
