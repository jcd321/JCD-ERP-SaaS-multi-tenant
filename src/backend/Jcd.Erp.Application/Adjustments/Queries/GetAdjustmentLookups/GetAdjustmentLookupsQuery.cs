using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Adjustments.Queries.GetAdjustmentLookups;

public record GetAdjustmentLookupsQuery : IRequest<Result<AdjustmentLookupsResult>>;

public record AdjustmentStockLevelDto(Guid ProductId, Guid WarehouseId, decimal QuantityOnHand);

public record AdjustmentLookupsResult(
    IReadOnlyList<LookupOptionDto> Products,
    IReadOnlyList<LookupOptionDto> Warehouses,
    IReadOnlyList<AdjustmentStockLevelDto> StockLevels);
