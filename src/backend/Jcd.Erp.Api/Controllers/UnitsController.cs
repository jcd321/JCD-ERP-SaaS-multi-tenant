using Jcd.Erp.Application.Units.Commands.CreateUnit;
using Jcd.Erp.Application.Units.Commands.DeleteUnit;
using Jcd.Erp.Application.Units.Commands.UpdateUnit;
using Jcd.Erp.Application.Units.Queries.GetUnits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/units")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly ISender _mediator;

    public UnitsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "units.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetUnitsQuery(page, pageSize, search, isActive));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "units.create")]
    public async Task<IActionResult> Create([FromBody] CreateUnitCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "units.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitRequest request)
    {
        var command = new UpdateUnitCommand(id, request.Code, request.Name, request.Symbol, request.IsActive);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "units.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteUnitCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}

public record UpdateUnitRequest(
    string Code,
    string Name,
    string? Symbol,
    bool IsActive);
