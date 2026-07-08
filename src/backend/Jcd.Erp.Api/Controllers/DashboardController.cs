using Jcd.Erp.Application.Dashboard.Queries.GetDashboardKpis;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ISender _mediator;

    public DashboardController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis()
    {
        var result = await _mediator.Send(new GetDashboardKpisQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
