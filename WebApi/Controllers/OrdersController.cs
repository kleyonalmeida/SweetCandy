using Application.Features.Orders.Commands;
using Application.Features.Orders.DTOs;
using Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class OrdersController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetOrdersQuery(page, pageSize)));

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
    => FromResponse(await _sender.Send(new GetOrderByIdQuery(id)));

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    => FromResponse(await _sender.Send(new CreateOrderCommand(request)));

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateOrderRequest request)
    => FromResponse(await _sender.Send(new UpdateOrderCommand(id, request)));

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id)
    => FromResponse(await _sender.Send(new DeleteOrderCommand(id)));
}