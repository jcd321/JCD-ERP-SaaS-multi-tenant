using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Commands.DeleteStockLevel;

public class DeleteStockLevelHandler : IRequestHandler<DeleteStockLevelCommand, Result>
{
    private readonly IStockLevelRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStockLevelHandler(
        IStockLevelRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteStockLevelCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var stockLevel = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (stockLevel is null)
            return Result.Failure("StockLevel.NotFound");

        if (stockLevel.QuantityOnHand > 0)
            return Result.Failure("StockLevel.HasQuantity");

        _repository.Delete(stockLevel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
