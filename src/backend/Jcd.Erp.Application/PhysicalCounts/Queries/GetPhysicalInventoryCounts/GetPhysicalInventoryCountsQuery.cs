using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCounts;

public record GetPhysicalInventoryCountsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? WarehouseId = null,
    string? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<Result<PaginatedPhysicalInventoryCountsResult>>;

public record PhysicalInventoryCountLineDto(
    Guid Id,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    string? UnitSymbol,
    decimal SystemQuantity,
    decimal? CountedQuantity,
    int LineNumber,
    bool HasVariance);

public record PhysicalInventoryCountDto(
    Guid Id,
    string DocumentNumber,
    Guid WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    DateTime CountDate,
    string Status,
    string? Notes,
    int LineCount,
    int CountedLineCount,
    int VarianceLineCount,
    IReadOnlyList<PhysicalInventoryCountLineDto> Lines,
    DateTime CreatedAt,
    DateTime? CompletedAt);

public record PaginatedPhysicalInventoryCountsResult(
    IReadOnlyList<PhysicalInventoryCountDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
