using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Adjustments.Queries.GetInventoryAdjustments;

public record GetInventoryAdjustmentsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? WarehouseId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<Result<PaginatedInventoryAdjustmentsResult>>;

public record InventoryAdjustmentLineDto(
    Guid Id,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    string? UnitSymbol,
    decimal QuantityBefore,
    decimal QuantityAfter,
    int LineNumber);

public record InventoryAdjustmentDto(
    Guid Id,
    string DocumentNumber,
    Guid WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    DateTime AdjustmentDate,
    string Reason,
    string? Notes,
    int LineCount,
    IReadOnlyList<InventoryAdjustmentLineDto> Lines,
    DateTime CreatedAt);

public record PaginatedInventoryAdjustmentsResult(
    IReadOnlyList<InventoryAdjustmentDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
