using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Kardex.Queries.GetKardexLookups;

public class GetKardexLookupsHandler : IRequestHandler<GetKardexLookupsQuery, Result<StockLookupsResult>>
{
    private readonly ISender _mediator;
    private readonly ICurrentTenantService _tenant;

    public GetKardexLookupsHandler(ISender mediator, ICurrentTenantService tenant)
    {
        _mediator = mediator;
        _tenant = tenant;
    }

    public async Task<Result<StockLookupsResult>> Handle(
        GetKardexLookupsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<StockLookupsResult>("Auth.TenantRequired");

        return await _mediator.Send(new GetStockLookupsQuery(), cancellationToken);
    }
}
