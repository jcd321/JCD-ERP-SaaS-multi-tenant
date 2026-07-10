using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Transfers.Queries.GetTransferLookups;

public record GetTransferLookupsQuery : IRequest<Result<TransferLookupsResult>>;

public record TransferStockLevelDto(Guid ProductId, Guid WarehouseId, decimal QuantityOnHand);

public record TransferLookupsResult(
    IReadOnlyList<LookupOptionDto> Products,
    IReadOnlyList<LookupOptionDto> Warehouses,
    IReadOnlyList<TransferStockLevelDto> StockLevels);
