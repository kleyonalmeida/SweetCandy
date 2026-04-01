using Application.Features.MonthlyGoals.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.MonthlyGoals.Commands;

public class UpsertMonthlyGoalCommand(UpsertMonthlyGoalRequest request) : IRequest<IResponseWrapper>, IValidateMe
{
  public UpsertMonthlyGoalRequest Request { get; set; } = request;
}

public class UpsertMonthlyGoalCommandHandler(IMonthlyGoalService monthlyGoalService) : IRequestHandler<UpsertMonthlyGoalCommand, IResponseWrapper>
{
  private readonly IMonthlyGoalService _monthlyGoalService = monthlyGoalService;

  public async Task<IResponseWrapper> Handle(UpsertMonthlyGoalCommand request, CancellationToken cancellationToken)
  {
    var goal = request.Request.Adapt<MonthlyGoal>();
    var err = await _monthlyGoalService.UpsertAsync(goal);
    if (!string.IsNullOrWhiteSpace(err))
      return await ResponseWrapper.FailAsync(err);
    return await ResponseWrapper.SuccessAsync("Meta mensal salva com sucesso.");
  }
}
