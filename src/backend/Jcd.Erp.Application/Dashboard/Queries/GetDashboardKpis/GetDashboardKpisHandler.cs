using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Inventory.Warehouses;
using MediatR;

namespace Jcd.Erp.Application.Dashboard.Queries.GetDashboardKpis;

public class GetDashboardKpisHandler : IRequestHandler<GetDashboardKpisQuery, Result<DashboardKpisDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockLevelRepository _stockRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;

    public GetDashboardKpisHandler(
        IProductRepository productRepository,
        IStockLevelRepository stockRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant)
    {
        _productRepository = productRepository;
        _stockRepository = stockRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
    }

    public async Task<Result<DashboardKpisDto>> Handle(
        GetDashboardKpisQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<DashboardKpisDto>("Auth.TenantRequired");

        var (_, productsCount) = await _productRepository.GetPagedAsync(1, 1, null, true, cancellationToken);
        var (_, warehousesCount) = await _warehouseRepository.GetPagedAsync(1, 1, null, true, cancellationToken);
        var (stockItems, stockRecordsCount) = await _stockRepository.GetPagedAsync(
            1,
            500,
            null,
            null,
            null,
            null,
            cancellationToken);
        var (_, belowMinimumCount) = await _stockRepository.GetPagedAsync(
            1,
            1,
            null,
            null,
            null,
            true,
            cancellationToken);

        var totalQuantity = stockItems.Sum(s => s.QuantityOnHand);

        return Result.Success(new DashboardKpisDto(
            productsCount,
            stockRecordsCount,
            totalQuantity,
            belowMinimumCount,
            warehousesCount,
            OrdersCount: null,
            Revenue: null));
    }
}
