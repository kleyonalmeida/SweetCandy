using Application.Features.Expenses;
using Application.Features.MonthlyGoals.DTOs;
using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using MediatR;

namespace Application.Features.MonthlyGoals.Commands;

public class UpsertMonthlyGoalCommand(UpsertMonthlyGoalRequest request) : IRequest<IResponseWrapper>, IValidateMe
{
  public UpsertMonthlyGoalRequest Request { get; set; } = request;
}

public class UpsertMonthlyGoalCommandHandler(
  IMonthlyGoalService monthlyGoalService,
  IExpenseService expenseService) : IRequestHandler<UpsertMonthlyGoalCommand, IResponseWrapper>
{
  private readonly IMonthlyGoalService _monthlyGoalService = monthlyGoalService;
  private readonly IExpenseService _expenseService = expenseService;

  public async Task<IResponseWrapper> Handle(UpsertMonthlyGoalCommand request, CancellationToken cancellationToken)
  {
    decimal targetAmount;

    if (request.Request.PercentageOverCosts.HasValue)
    {
      var year = request.Request.Year;
      var month = request.Request.Month;
      var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
      var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

      var allExpenses = await _expenseService.GetAllAsync();
      var monthlyExpenses = allExpenses
        .Where(e => e.Date >= monthStart && e.Date <= monthEnd)
        .Sum(e => e.Value ?? 0m);

      targetAmount = Math.Round(monthlyExpenses * (1m + request.Request.PercentageOverCosts.Value / 100m), 2);
    }
    else
    {
      targetAmount = request.Request.TargetAmount!.Value;
    }

    var goal = new MonthlyGoal
    {
      Year = request.Request.Year,
      Month = request.Request.Month,
      TargetAmount = targetAmount
    };

    var err = await _monthlyGoalService.UpsertAsync(goal);
    if (!string.IsNullOrWhiteSpace(err))
      return await ResponseWrapper.FailAsync(err);
    return await ResponseWrapper.SuccessAsync("Meta mensal salva com sucesso.");
  }
}
