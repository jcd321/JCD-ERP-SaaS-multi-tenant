using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Customers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Customers.Queries.GetCustomers;

public class GetCustomersHandler : IRequestHandler<GetCustomersQuery, Result<PaginatedCustomersResult>>
{
    private readonly ICustomerRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetCustomersHandler(ICustomerRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedCustomersResult>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedCustomersResult>("Auth.TenantRequired");

        var page = Math.Clamp(request.Page, 1, int.MaxValue);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = items
            .Select(c => new CustomerDto(
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

        return Result.Success(new PaginatedCustomersResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
