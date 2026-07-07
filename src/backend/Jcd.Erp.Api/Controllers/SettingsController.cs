using Jcd.Erp.Application.Settings.Commands.UpdateTenantSetting;
using Jcd.Erp.Application.Settings.Queries.GetTenantSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ISender _mediator;

    public SettingsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "settings.view")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetTenantSettingsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{key}")]
    [Authorize(Policy = "settings.update")]
    public async Task<IActionResult> Update(string key, [FromBody] UpdateSettingRequest request)
    {
        var result = await _mediator.Send(new UpdateTenantSettingCommand(key, request.Value));
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }
}

public record UpdateSettingRequest(string Value);
