using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Commands.UpdateStockLevel;

public class UpdateStockLevelHandler : IRequestHandler<UpdateStockLevelCommand, Result>
{
    private readonly IStockLevelRepository _repository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStockLevelHandler(
        IStockLevelRepository repository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateStockLevelCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure("Auth.TenantRequired");

        var stockLevel = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (stockLevel is null)
            return Result.Failure("StockLevel.NotFound");

        var updateResult = stockLevel.UpdateLevels(
            request.QuantityOnHand,
            request.MinQuantity,
            request.MaxQuantity);

        if (updateResult.IsFailure)
            return updateResult;

        _repository.Update(stockLevel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
