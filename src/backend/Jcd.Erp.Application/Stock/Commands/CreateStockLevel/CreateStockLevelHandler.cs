using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Commands.CreateStockLevel;

public class CreateStockLevelHandler : IRequestHandler<CreateStockLevelCommand, Result<Guid>>
{
    private readonly IStockLevelRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStockLevelHandler(
        IStockLevelRepository stockRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant,
        IUnitOfWork unitOfWork)
    {
        _stockRepository = stockRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateStockLevelCommand request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<Guid>("Auth.TenantRequired");

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<Guid>("StockLevel.ProductNotFound");

        if (!product.IsActive)
            return Result.Failure<Guid>("StockLevel.ProductInactive");

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse is null)
            return Result.Failure<Guid>("StockLevel.WarehouseNotFound");

        if (!warehouse.IsActive)
            return Result.Failure<Guid>("StockLevel.WarehouseInactive");

        if (await _stockRepository.GetByProductAndWarehouseAsync(
                request.ProductId,
                request.WarehouseId,
                cancellationToken) is not null)
            return Result.Failure<Guid>("StockLevel.AlreadyExists");

        var stockResult = StockLevel.Create(
            _tenant.TenantId,
            request.ProductId,
            request.WarehouseId,
            request.QuantityOnHand,
            request.MinQuantity,
            request.MaxQuantity);

        if (stockResult.IsFailure)
            return Result.Failure<Guid>(stockResult.Error);

        var stockLevel = stockResult.Value;
        await _stockRepository.AddAsync(stockLevel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(stockLevel.Id);
    }
}
