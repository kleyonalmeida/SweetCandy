using Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
  protected IActionResult FromResponse(IResponseWrapper response)
  {
    return response.IsSuccessful ? Ok(response) : BadRequest(response);
  }
}