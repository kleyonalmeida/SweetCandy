using Application.Features.StockMovements.Commands;
using Application.Features.StockMovements.DTOs;
using Application.Features.StockMovements.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class StockMovementsController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetStockMovementsQuery(page, pageSize)));

  [HttpGet("GetBySupplyId/{supplyId}")]
  public async Task<IActionResult> GetBySupplyId(string supplyId)
    => FromResponse(await _sender.Send(new GetStockMovementsBySupplyIdQuery(supplyId)));

  [HttpGet("GetByOrderId/{orderId}")]
  public async Task<IActionResult> GetByOrderId(string orderId)
    => FromResponse(await _sender.Send(new GetStockMovementsByOrderIdQuery(orderId)));

  [HttpPost("Create")]
  public async Task<IActionResult> Create([FromBody] CreateStockMovementRequest request)
    => FromResponse(await _sender.Send(new CreateStockMovementCommand(request)));
}
