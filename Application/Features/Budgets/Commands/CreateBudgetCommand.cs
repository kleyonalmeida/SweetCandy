using Application.Features.Budget.DTOs;
using Application.Wrappers;
using MediatR;
using Application.Pipelines;

namespace Application.Features.Budget.Commands;

public class CreateBudgetCommand(CreateBudgetRequest createBudget) : IRequest<IResponseWrapper>, IValidateMe
{
  public CreateBudgetRequest CreateBudget { get; set; } = createBudget;
}
