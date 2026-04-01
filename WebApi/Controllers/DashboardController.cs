using Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class DashboardController(ISender sender) : ApiControllerBase
{
  private readonly ISender _sender = sender;

  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] int? year = null, [FromQuery] int? month = null)
    => FromResponse(await _sender.Send(new GetDashboardQuery(year, month)));
}
