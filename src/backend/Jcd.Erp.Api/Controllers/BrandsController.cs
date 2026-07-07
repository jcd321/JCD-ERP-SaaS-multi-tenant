using Jcd.Erp.Api.Requests;
using Jcd.Erp.Application.Brands.Commands.CreateBrand;
using Jcd.Erp.Application.Brands.Commands.DeleteBrand;
using Jcd.Erp.Application.Brands.Commands.UpdateBrand;
using Jcd.Erp.Application.Brands.Queries.GetBrands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/brands")]
[Authorize]
public class BrandsController : ControllerBase
{
    private readonly ISender _mediator;

    public BrandsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "brands.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetBrandsQuery(page, pageSize, search, isActive));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "brands.create")]
    public async Task<IActionResult> Create([FromBody] CreateBrandCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "brands.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandRequest request)
    {
        var command = new UpdateBrandCommand(id, request.Code, request.Name, request.Description, request.IsActive);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "brands.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBrandCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
