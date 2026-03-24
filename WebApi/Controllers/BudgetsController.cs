using Application.Features.Budgets.Commands;
using Application.Features.Budgets.DTOs;
using Application.Features.Budgets.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class BudgetsController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetBudgetsQuery(page, pageSize)));

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
    => FromResponse(await _sender.Send(new GetBudgetByIdQuery(id)));

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateBudgetRequest request)
    => FromResponse(await _sender.Send(new CreateBudgetCommand(request)));

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateBudgetRequest request)
    => FromResponse(await _sender.Send(new UpdateBudgetCommand(id, request)));

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id)
    => FromResponse(await _sender.Send(new DeleteBudgetCommand(id)));
}