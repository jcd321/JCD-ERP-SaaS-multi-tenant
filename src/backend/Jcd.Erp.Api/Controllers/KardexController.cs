using Jcd.Erp.Application.Kardex.Queries.GetKardex;
using Jcd.Erp.Application.Kardex.Queries.GetKardexLookups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/kardex")]
[Authorize]
public class KardexController : ControllerBase
{
    private readonly ISender _mediator;

    public KardexController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "kardex.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(new GetKardexQuery(
            productId,
            page,
            pageSize,
            warehouseId,
            fromDate,
            toDate));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("lookups")]
    [Authorize(Policy = "kardex.view")]
    public async Task<IActionResult> GetLookups()
    {
        var result = await _mediator.Send(new GetKardexLookupsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
