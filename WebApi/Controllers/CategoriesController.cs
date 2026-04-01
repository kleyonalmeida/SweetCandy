using Application.Features.Categories.Commands;
using Application.Features.Categories.DTOs;
using Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    => FromResponse(await _sender.Send(new GetCategoriesQuery(page, pageSize)));

  [HttpGet("GetById/{id}")]
  public async Task<IActionResult> GetById(string id)
    => FromResponse(await _sender.Send(new GetCategoryByIdQuery(id)));

  [HttpPost("Create")]
  public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    => FromResponse(await _sender.Send(new CreateCategoryCommand(request)));

  [HttpPut("Update/{id}")]
  public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryRequest request)
    => FromResponse(await _sender.Send(new UpdateCategoryCommand(id, request)));

  [HttpDelete("Delete/{id}")]
  public async Task<IActionResult> Delete(string id)
    => FromResponse(await _sender.Send(new DeleteCategoryCommand(id)));
}
