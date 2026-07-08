using Jcd.Erp.Api.Requests;
using Jcd.Erp.Application.Suppliers.Commands.CreateSupplier;
using Jcd.Erp.Application.Suppliers.Commands.DeleteSupplier;
using Jcd.Erp.Application.Suppliers.Commands.UpdateSupplier;
using Jcd.Erp.Application.Suppliers.Queries.GetSuppliers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jcd.Erp.Api.Controllers;

[ApiController]
[Route("api/v1/suppliers")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISender _mediator;

    public SuppliersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "suppliers.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetSuppliersQuery(page, pageSize, search, isActive));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "suppliers.create")]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "suppliers.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierRequest request)
    {
        var command = new UpdateSupplierCommand(
            id,
            request.Code,
            request.LegalName,
            request.TradeName,
            request.TaxId,
            request.Email,
            request.Phone,
            request.MobilePhone,
            request.AddressLine1,
            request.City,
            request.StateOrProvince,
            request.CountryCode,
            request.Notes,
            request.IsActive);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "suppliers.delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteSupplierCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
