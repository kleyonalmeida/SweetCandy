using Application.Features.MonthlyGoals.DTOs;
using Application.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.MonthlyGoals.Queries;

public record GetMonthlyGoalByMonthQuery(int Year, int Month) : IRequest<ResponseWrapper<MonthlyGoalResponse?>>;

public class GetMonthlyGoalByMonthQueryHandler(IMonthlyGoalService monthlyGoalService)
  : IRequestHandler<GetMonthlyGoalByMonthQuery, ResponseWrapper<MonthlyGoalResponse?>>
{
  private readonly IMonthlyGoalService _monthlyGoalService = monthlyGoalService;

  public async Task<ResponseWrapper<MonthlyGoalResponse?>> Handle(GetMonthlyGoalByMonthQuery request, CancellationToken cancellationToken)
  {
    var goal = await _monthlyGoalService.GetByMonthAsync(request.Year, request.Month);
    if (goal is null)
      return await ResponseWrapper<MonthlyGoalResponse?>.SuccessAsync((MonthlyGoalResponse?)null);
    return await ResponseWrapper<MonthlyGoalResponse?>.SuccessAsync(goal.Adapt<MonthlyGoalResponse>());
  }
}
