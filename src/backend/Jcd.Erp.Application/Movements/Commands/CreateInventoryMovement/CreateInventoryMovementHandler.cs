using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Inventory.Movements;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Movements.Commands.CreateInventoryMovement;

public class CreateInventoryMovementHandler : IRequestHandler<CreateInventoryMovementCommand, Result<Guid>>
{
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IStockLevelRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryMovementHandler(
        IInventoryMovementRepository movementRepository,
        IStockLevelRepository stockRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _movementRepository = movementRepository;
        _stockRepository = stockRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateInventoryMovementCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        if (!InventoryMovementTypes.IsValid(request.MovementType))
            return Result.Failure<Guid>("Movement.TypeInvalid");

        var movementType = InventoryMovementTypes.Normalize(request.MovementType);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<Guid>("Movement.ProductNotFound");

        if (!product.IsActive)
            return Result.Failure<Guid>("Movement.ProductInactive");

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse is null)
            return Result.Failure<Guid>("Movement.WarehouseNotFound");

        if (!warehouse.IsActive)
            return Result.Failure<Guid>("Movement.WarehouseInactive");

        var stockLevel = await _stockRepository.GetByProductAndWarehouseAsync(
            request.ProductId,
            request.WarehouseId,
            cancellationToken);

        if (stockLevel is null)
        {
            if (movementType == InventoryMovementTypes.Out)
                return Result.Failure<Guid>("Movement.StockNotFound");

            var stockResult = StockLevel.Create(
                _tenant.TenantId,
                request.ProductId,
                request.WarehouseId,
                0);

            if (stockResult.IsFailure)
                return Result.Failure<Guid>(stockResult.Error);

            stockLevel = stockResult.Value;
            await _stockRepository.AddAsync(stockLevel, cancellationToken);
        }

        var quantityChange = movementType == InventoryMovementTypes.In
            ? request.Quantity
            : -request.Quantity;

        var quantityBefore = stockLevel.QuantityOnHand;
        var applyResult = stockLevel.ApplyQuantityChange(quantityChange);
        if (applyResult.IsFailure)
            return Result.Failure<Guid>(applyResult.Error);

        var documentNumber = await _movementRepository.GetNextDocumentNumberAsync(cancellationToken);
        var movementDate = request.MovementDate ?? DateTime.UtcNow;

        var movementResult = InventoryMovement.Create(
            _tenant.TenantId,
            documentNumber,
            request.ProductId,
            request.WarehouseId,
            movementType,
            request.Quantity,
            quantityBefore,
            stockLevel.QuantityOnHand,
            movementDate,
            request.Reference,
            request.Notes);

        if (movementResult.IsFailure)
            return Result.Failure<Guid>(movementResult.Error);

        _stockRepository.Update(stockLevel);
        await _movementRepository.AddAsync(movementResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(movementResult.Value.Id);
    }
}
