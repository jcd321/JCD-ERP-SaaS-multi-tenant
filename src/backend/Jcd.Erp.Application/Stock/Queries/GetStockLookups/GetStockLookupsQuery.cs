using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Queries.GetStockLookups;

public record GetStockLookupsQuery : IRequest<Result<StockLookupsResult>>;

public record LookupOptionDto(Guid Id, string Label);

public record StockLookupsResult(
    IReadOnlyList<LookupOptionDto> Products,
    IReadOnlyList<LookupOptionDto> Warehouses);
