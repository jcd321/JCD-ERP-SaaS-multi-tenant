using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Suppliers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Suppliers.Queries.GetSuppliers;

public class GetSuppliersHandler : IRequestHandler<GetSuppliersQuery, Result<PaginatedSuppliersResult>>
{
    private readonly ISupplierRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetSuppliersHandler(ISupplierRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedSuppliersResult>> Handle(
        GetSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedSuppliersResult>("Auth.TenantRequired");

        var page = Math.Clamp(request.Page, 1, int.MaxValue);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(c => new SupplierDto(
                c.Id,
                c.Code,
                c.LegalName,
                c.TradeName,
                c.TaxId,
                c.Email,
                c.Phone,
                c.MobilePhone,
                c.AddressLine1,
                c.City,
                c.StateOrProvince,
                c.CountryCode,
                c.Notes,
                c.IsActive))
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result.Success(new PaginatedSuppliersResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
