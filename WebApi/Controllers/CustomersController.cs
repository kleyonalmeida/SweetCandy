using Application.Features.Customers.Commands;
using Application.Features.Customers.DTOs;
using Application.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class CustomersController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetCustomersQuery(page, pageSize)));

  [HttpGet("GetById/{id}")]
  public async Task<IActionResult> GetById(string id)
    => FromResponse(await _sender.Send(new GetCustomerByIdQuery(id)));

  [HttpPost("Create")]
  public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    => FromResponse(await _sender.Send(new CreateCustomerCommand(request)));

  [HttpPut("Update/{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerRequest request)
    => FromResponse(await _sender.Send(new UpdateCustomerCommand(id, request)));

  [HttpDelete("Delete/{id}")]
  public async Task<IActionResult> Delete(string id)
    => FromResponse(await _sender.Send(new DeleteCustomerCommand(id)));
}
