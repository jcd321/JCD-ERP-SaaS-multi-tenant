using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Units;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Queries.GetUnits;

public class GetUnitsHandler : IRequestHandler<GetUnitsQuery, Result<PaginatedUnitsResult>>
{
    private readonly IUnitOfMeasureRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetUnitsHandler(IUnitOfMeasureRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedUnitsResult>> Handle(
        GetUnitsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedUnitsResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(u => new UnitOfMeasureDto(u.Id, u.Code, u.Name, u.Symbol, u.IsActive))
            .ToList();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedUnitsResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
