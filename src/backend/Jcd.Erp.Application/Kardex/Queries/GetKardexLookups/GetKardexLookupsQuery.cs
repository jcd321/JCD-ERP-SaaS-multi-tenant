using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Kardex.Queries.GetKardexLookups;

public record GetKardexLookupsQuery : IRequest<Result<StockLookupsResult>>;
