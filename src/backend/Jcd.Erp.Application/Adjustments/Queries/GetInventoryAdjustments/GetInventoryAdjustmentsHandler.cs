using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Adjustments;
using MediatR;

namespace Jcd.Erp.Application.Adjustments.Queries.GetInventoryAdjustments;

public class GetInventoryAdjustmentsHandler : IRequestHandler<GetInventoryAdjustmentsQuery, Result<PaginatedInventoryAdjustmentsResult>>
{
    private readonly IInventoryAdjustmentRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetInventoryAdjustmentsHandler(IInventoryAdjustmentRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedInventoryAdjustmentsResult>> Handle(
        GetInventoryAdjustmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedInventoryAdjustmentsResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.WarehouseId,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedInventoryAdjustmentsResult(dtos, page, pageSize, totalCount, totalPages));
    }

    private static InventoryAdjustmentDto MapToDto(InventoryAdjustment adjustment)
    {
        var lines = adjustment.Lines
            .OrderBy(l => l.LineNumber)
            .Select(l => new InventoryAdjustmentLineDto(
                l.Id,
                l.ProductId,
                l.Product.Sku,
                l.Product.Name,
                l.Product.Unit.Symbol,
                l.QuantityBefore,
                l.QuantityAfter,
                l.LineNumber))
            .ToList();

        return new InventoryAdjustmentDto(
            adjustment.Id,
            adjustment.DocumentNumber,
            adjustment.WarehouseId,
            adjustment.Warehouse.Code,
            adjustment.Warehouse.Name,
            adjustment.AdjustmentDate,
            adjustment.Reason,
            adjustment.Notes,
            lines.Count,
            lines,
            adjustment.CreatedAt);
    }
}
