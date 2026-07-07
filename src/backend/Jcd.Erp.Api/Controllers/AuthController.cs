using Jcd.Erp.Application.Auth.Commands.ChangePassword;
using Jcd.Erp.Application.Auth.Commands.Login;
using Jcd.Erp.Application.Auth.Commands.Logout;
using Jcd.Erp.Application.Auth.Commands.RefreshToken;
using Jcd.Erp.Application.Auth.Commands.RegisterTenant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterTenantCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}
