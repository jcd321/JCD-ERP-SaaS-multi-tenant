using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Transfers.Queries.GetInventoryTransfers;

public record GetInventoryTransfersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? SourceWarehouseId = null,
    Guid? DestinationWarehouseId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<Result<PaginatedInventoryTransfersResult>>;

public record InventoryTransferLineDto(
    Guid Id,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    string? UnitSymbol,
    decimal Quantity,
    int LineNumber);

public record InventoryTransferDto(
    Guid Id,
    string DocumentNumber,
    Guid SourceWarehouseId,
    string SourceWarehouseCode,
    string SourceWarehouseName,
    Guid DestinationWarehouseId,
    string DestinationWarehouseCode,
    string DestinationWarehouseName,
    DateTime TransferDate,
    string? Notes,
    int LineCount,
    IReadOnlyList<InventoryTransferLineDto> Lines,
    DateTime CreatedAt);

public record PaginatedInventoryTransfersResult(
    IReadOnlyList<InventoryTransferDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
