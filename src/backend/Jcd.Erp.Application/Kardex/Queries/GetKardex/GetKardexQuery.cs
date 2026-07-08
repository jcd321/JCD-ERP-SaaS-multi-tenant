using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Kardex.Queries.GetKardex;

public record GetKardexQuery(
    Guid ProductId,
    int Page = 1,
    int PageSize = 50,
    Guid? WarehouseId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<Result<PaginatedKardexResult>>;

public record KardexEntryDto(
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

public record PaginatedKardexResult(
    IReadOnlyList<KardexEntryDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
