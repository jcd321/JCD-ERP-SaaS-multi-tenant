using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Warehouses;
using MediatR;

namespace Jcd.Erp.Application.Adjustments.Queries.GetAdjustmentLookups;

public class GetAdjustmentLookupsHandler : IRequestHandler<GetAdjustmentLookupsQuery, Result<AdjustmentLookupsResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStockLevelRepository _stockRepository;
    private readonly ICurrentTenantService _tenant;

    public GetAdjustmentLookupsHandler(
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IStockLevelRepository stockRepository,
        ICurrentTenantService tenant)
    {
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _stockRepository = stockRepository;
        _tenant = tenant;
    }

    public async Task<Result<AdjustmentLookupsResult>> Handle(
        GetAdjustmentLookupsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<AdjustmentLookupsResult>("Auth.TenantRequired");

        var (products, _) = await _productRepository.GetPagedAsync(1, 500, null, true, cancellationToken);
        var (warehouses, _) = await _warehouseRepository.GetPagedAsync(1, 500, null, true, cancellationToken);
        var (stockLevels, _) = await _stockRepository.GetPagedAsync(
            1,
            5000,
            null,
            null,
            null,
            null,
            cancellationToken);

        return Result.Success(new AdjustmentLookupsResult(
            products.Select(p => new LookupOptionDto(p.Id, $"{p.Sku} — {p.Name}")).ToList(),
            warehouses.Select(w => new LookupOptionDto(w.Id, $"{w.Code} — {w.Name}")).ToList(),
            stockLevels
                .Select(s => new AdjustmentStockLevelDto(s.ProductId, s.WarehouseId, s.QuantityOnHand))
                .ToList()));
    }
}
