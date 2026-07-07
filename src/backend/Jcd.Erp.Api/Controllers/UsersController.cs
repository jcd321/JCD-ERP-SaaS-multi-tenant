using Jcd.Erp.Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "users.view")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
