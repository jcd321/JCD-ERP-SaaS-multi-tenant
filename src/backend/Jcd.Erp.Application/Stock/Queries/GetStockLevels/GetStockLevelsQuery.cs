using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Queries.GetStockLevels;

public record GetStockLevelsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? WarehouseId = null,
    Guid? ProductId = null,
    bool? BelowMinimumOnly = null) : IRequest<Result<PaginatedStockLevelsResult>>;

public record StockLevelDto(
    Guid Id,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    string? UnitSymbol,
    Guid WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    decimal QuantityOnHand,
    decimal? MinQuantity,
    decimal? MaxQuantity,
    bool IsBelowMinimum);

public record PaginatedStockLevelsResult(
    IReadOnlyList<StockLevelDto> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
