using Application.Features.Expenses.Commands;
using Application.Features.Expenses.DTOs;
using Application.Features.Expenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ExpensesController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetExpensesQuery(page, pageSize)));

  [HttpGet("GetById/{id}")]
  public async Task<IActionResult> GetById(string id)
    => FromResponse(await _sender.Send(new GetExpenseByIdQuery(id)));

  [HttpPost("Create")]
  public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
    => FromResponse(await _sender.Send(new CreateExpenseCommand(request)));

  [HttpPut("Update/{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateExpenseRequest request)
    => FromResponse(await _sender.Send(new UpdateExpenseCommand(id, request)));

  [HttpDelete("Delete/{id}")]
  public async Task<IActionResult> Delete(string id)
    => FromResponse(await _sender.Send(new DeleteExpenseCommand(id)));
}
