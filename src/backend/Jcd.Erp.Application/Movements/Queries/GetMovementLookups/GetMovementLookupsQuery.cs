using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Movements.Queries.GetMovementLookups;

public record GetMovementLookupsQuery : IRequest<Result<StockLookupsResult>>;
