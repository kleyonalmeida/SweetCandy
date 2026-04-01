using Application.Features.MonthlyGoals.Commands;
using Application.Features.MonthlyGoals.DTOs;
using Application.Features.MonthlyGoals.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class MonthlyGoalsController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet]
  public async Task<IActionResult> GetByMonth([FromQuery] int year, [FromQuery] int month)
    => FromResponse(await _sender.Send(new GetMonthlyGoalByMonthQuery(year, month)));

  [HttpPost]
  public async Task<IActionResult> Upsert([FromBody] UpsertMonthlyGoalRequest request)
    => FromResponse(await _sender.Send(new UpsertMonthlyGoalCommand(request)));
}
