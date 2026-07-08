using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Suppliers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand, Result>
{
    private readonly ISupplierRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSupplierHandler(
        ISupplierRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return Result.Failure("Supplier.NotFound");

        _repository.Delete(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
