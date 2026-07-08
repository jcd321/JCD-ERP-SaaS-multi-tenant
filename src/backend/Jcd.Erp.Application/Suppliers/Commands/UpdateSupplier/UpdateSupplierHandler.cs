using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Suppliers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand, Result>
{
    private readonly ISupplierRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierHandler(
        ISupplierRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return Result.Failure("Supplier.NotFound");

        var existingByCode = await _repository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingByCode is not null && existingByCode.Id != supplier.Id)
            return Result.Failure("Supplier.CodeAlreadyExists");

        if (!string.IsNullOrWhiteSpace(request.TaxId))
        {
            var existingByTaxId = await _repository.GetByTaxIdAsync(request.TaxId, cancellationToken);
            if (existingByTaxId is not null && existingByTaxId.Id != supplier.Id)
                return Result.Failure("Supplier.TaxIdAlreadyExists");
        }

        var updateResult = supplier.Update(
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

        _repository.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
