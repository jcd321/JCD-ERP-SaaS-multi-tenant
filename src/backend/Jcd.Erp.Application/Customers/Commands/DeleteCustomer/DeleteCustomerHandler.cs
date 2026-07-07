using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Partners.Customers;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Customers.Commands.DeleteCustomer;

public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly ICustomerRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerHandler(
        ICustomerRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
            return Result.Failure("Customer.NotFound");

        _repository.Delete(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
