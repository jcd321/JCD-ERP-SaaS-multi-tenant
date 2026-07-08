using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Movements.Queries.GetInventoryMovements;

public record GetInventoryMovementsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? WarehouseId = null,
    Guid? ProductId = null,
    string? MovementType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<Result<PaginatedInventoryMovementsResult>>;

public record InventoryMovementDto(
    Guid Id,
    string DocumentNumber,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    string? UnitSymbol,
    Guid WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    string MovementType,
    decimal Quantity,
    decimal QuantityBefore,
    decimal QuantityAfter,
    string? Reference,
    string? Notes,
    DateTime MovementDate,
    DateTime CreatedAt);

public record PaginatedInventoryMovementsResult(
    IReadOnlyList<InventoryMovementDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
