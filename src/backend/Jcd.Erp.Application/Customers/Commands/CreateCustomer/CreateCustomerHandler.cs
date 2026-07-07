using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Customers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(
        ICustomerRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        if (await _repository.GetByCodeAsync(request.Code, cancellationToken) is not null)
            return Result.Failure<Guid>("Customer.CodeAlreadyExists");

        if (!string.IsNullOrWhiteSpace(request.TaxId) &&
            await _repository.GetByTaxIdAsync(request.TaxId, cancellationToken) is not null)
        {
            return Result.Failure<Guid>("Customer.TaxIdAlreadyExists");
        }

        var customerResult = Customer.Create(
            tenantId,
            request.Code,
            request.LegalName,
            request.TradeName,
            request.TaxId,
            request.Email,
            request.Phone,
            request.MobilePhone,
            request.AddressLine1,
            request.City,
            request.StateOrProvince,
            request.CountryCode,
            request.Notes);

        if (customerResult.IsFailure)
            return Result.Failure<Guid>(customerResult.Error);

        var customer = customerResult.Value;
        await _repository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.Id);
    }
}
