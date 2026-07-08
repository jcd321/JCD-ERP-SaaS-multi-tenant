using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Suppliers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Result<Guid>>
{
    private readonly ISupplierRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSupplierHandler(
        ISupplierRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var tenantId = _tenant.TenantId;

        if (await _repository.GetByCodeAsync(request.Code, cancellationToken) is not null)
            return Result.Failure<Guid>("Supplier.CodeAlreadyExists");

        if (!string.IsNullOrWhiteSpace(request.TaxId) &&
            await _repository.GetByTaxIdAsync(request.TaxId, cancellationToken) is not null)
        {
            return Result.Failure<Guid>("Supplier.TaxIdAlreadyExists");
        }

        var supplierResult = Supplier.Create(
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

        if (supplierResult.IsFailure)
            return Result.Failure<Guid>(supplierResult.Error);

        var supplier = supplierResult.Value;
        await _repository.AddAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(supplier.Id);
    }
}
