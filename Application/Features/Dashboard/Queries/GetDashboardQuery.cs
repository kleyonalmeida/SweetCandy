using Application.Features.Dashboard.DTOs;
using Application.Features.Expenses;
using Application.Features.MonthlyGoals;
using Application.Features.Receipts;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Dashboard.Queries;

public record GetDashboardQuery(int? Year = null, int? Month = null) : IRequest<ResponseWrapper<DashboardResponse>>;

public class GetDashboardQueryHandler(
  IReceiptsService receiptsService,
  IExpenseService expenseService,
  IMonthlyGoalService monthlyGoalService)
  : IRequestHandler<GetDashboardQuery, ResponseWrapper<DashboardResponse>>
{
  private readonly IReceiptsService _receiptsService = receiptsService;
  private readonly IExpenseService _expenseService = expenseService;
  private readonly IMonthlyGoalService _monthlyGoalService = monthlyGoalService;

  public async Task<ResponseWrapper<DashboardResponse>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
  {
    var now = DateTime.UtcNow;
    var year = request.Year ?? now.Year;
    var month = request.Month ?? now.Month;

    var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
    var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

    var receipts = await _receiptsService.GetAllAsync();
    var revenue = receipts
      .Where(r => r.Date >= monthStart && r.Date <= monthEnd)
      .Sum(r => r.Amount);

    var expensesList = await _expenseService.GetAllAsync();
    var expenses = expensesList
      .Where(e => e.Date >= monthStart && e.Date <= monthEnd)
      .Sum(e => e.Value ?? 0m);

    var profit = revenue - expenses;

    var goal = await _monthlyGoalService.GetByMonthAsync(year, month);
    decimal? goalPercent = null;
    if (goal is not null && goal.TargetAmount > 0)
      goalPercent = Math.Round(revenue / goal.TargetAmount * 100m, 2);

    var dto = new DashboardResponse
    {
      Year = year,
      Month = month,
      Revenue = revenue,
      Expenses = expenses,
      Profit = profit,
      MonthlyGoalTarget = goal?.TargetAmount,
      GoalProgressPercent = goalPercent
    };

    return await ResponseWrapper<DashboardResponse>.SuccessAsync(dto);
  }
}
