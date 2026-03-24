using Application.Features.Inventories.Commands;
using Application.Features.Inventories.DTOs;
using Application.Features.Inventories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class InventoriesController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet]
  public async Task<IActionResult> GetInventory()
    => FromResponse(await _sender.Send(new GetInventoryQuery()));

  [HttpGet("supplies")]
  public async Task<IActionResult> GetSupplies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetSuppliesQuery(page, pageSize)));

  [HttpGet("supplies/{id}")]
  public async Task<IActionResult> GetSupplyById(string id)
    => FromResponse(await _sender.Send(new GetSupplyByIdQuery(id)));

  [HttpPost("supplies")]
  public async Task<IActionResult> CreateSupply([FromBody] CreateSupplyRequest request)
    => FromResponse(await _sender.Send(new CreateSupplyCommand(request)));

  [HttpPut("supplies/{id}")]
  public async Task<IActionResult> UpdateSupply(string id, [FromBody] UpdateSupplyRequest request)
    => FromResponse(await _sender.Send(new UpdateSupplyCommand(id, request)));

  [HttpDelete("supplies/{id}")]
  public async Task<IActionResult> DeleteSupply(string id)
    => FromResponse(await _sender.Send(new DeleteSupplyCommand(id)));

  [HttpGet("final-products")]
  public async Task<IActionResult> GetFinalProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetFinalProductsQuery(page, pageSize)));

  [HttpGet("final-products/{id}")]
  public async Task<IActionResult> GetFinalProductById(string id)
    => FromResponse(await _sender.Send(new GetFinalProductByIdQuery(id)));

  [HttpPost("final-products")]
  public async Task<IActionResult> CreateFinalProduct([FromBody] CreateFinalProductRequest request)
    => FromResponse(await _sender.Send(new CreateFinalProductCommand(request)));

  [HttpPut("final-products/{id}")]
  public async Task<IActionResult> UpdateFinalProduct(string id, [FromBody] UpdateFinalProductRequest request)
    => FromResponse(await _sender.Send(new UpdateFinalProductCommand(id, request)));

  [HttpDelete("final-products/{id}")]
  public async Task<IActionResult> DeleteFinalProduct(string id)
    => FromResponse(await _sender.Send(new DeleteFinalProductCommand(id)));
}