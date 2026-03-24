using Application.Features.Receipts.Commands;
using Application.Features.Receipts.DTOs;
using Application.Features.Receipts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class ReceiptsController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetReceiptsQuery(page, pageSize)));

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
    => FromResponse(await _sender.Send(new GetReceiptByIdQuery(id)));

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateReceiptRequest request)
    => FromResponse(await _sender.Send(new CreateReceiptCommand(request)));

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateReceiptRequest request)
    => FromResponse(await _sender.Send(new UpdateReceiptCommand(id, request)));

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id)
    => FromResponse(await _sender.Send(new DeleteReceiptCommand(id)));
}