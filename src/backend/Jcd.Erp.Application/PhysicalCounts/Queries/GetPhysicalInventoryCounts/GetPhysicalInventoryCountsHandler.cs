using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.PhysicalCounts;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCounts;

public class GetPhysicalInventoryCountsHandler : IRequestHandler<GetPhysicalInventoryCountsQuery, Result<PaginatedPhysicalInventoryCountsResult>>
{
    private readonly IPhysicalInventoryCountRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetPhysicalInventoryCountsHandler(IPhysicalInventoryCountRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedPhysicalInventoryCountsResult>> Handle(
        GetPhysicalInventoryCountsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedPhysicalInventoryCountsResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.WarehouseId,
            request.Status,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = items.Select(PhysicalInventoryCountMapper.MapToDto).ToList();
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedPhysicalInventoryCountsResult(dtos, page, pageSize, totalCount, totalPages));
    }
}

internal static class PhysicalInventoryCountMapper
{
    public static PhysicalInventoryCountDto MapToDto(PhysicalInventoryCount count)
    {
        var lines = count.Lines
            .OrderBy(l => l.LineNumber)
            .Select(l => new PhysicalInventoryCountLineDto(
                l.Id,
                l.ProductId,
                l.Product?.Sku ?? string.Empty,
                l.Product?.Name ?? string.Empty,
                l.Product?.Unit?.Symbol,
                l.SystemQuantity,
                l.CountedQuantity,
                l.LineNumber,
                l.HasVariance))
            .ToList();

        return new PhysicalInventoryCountDto(
            count.Id,
            count.DocumentNumber,
            count.WarehouseId,
            count.Warehouse.Code,
            count.Warehouse.Name,
            count.CountDate,
            count.Status,
            count.Notes,
            lines.Count,
            lines.Count(l => l.CountedQuantity.HasValue),
            lines.Count(l => l.HasVariance),
            lines,
            count.CreatedAt,
            count.CompletedAt);
    }
}
