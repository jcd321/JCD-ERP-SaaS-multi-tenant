using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Customers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Customers.Commands.UpdateCustomer;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, Result>
{
    private readonly ICustomerRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(
        ICustomerRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
            return Result.Failure("Customer.NotFound");

        var existingByCode = await _repository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingByCode is not null && existingByCode.Id != customer.Id)
            return Result.Failure("Customer.CodeAlreadyExists");

        if (!string.IsNullOrWhiteSpace(request.TaxId))
        {
            var existingByTaxId = await _repository.GetByTaxIdAsync(request.TaxId, cancellationToken);
            if (existingByTaxId is not null && existingByTaxId.Id != customer.Id)
                return Result.Failure("Customer.TaxIdAlreadyExists");
        }

        var updateResult = customer.Update(
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
            request.Notes,
            request.IsActive);

        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
