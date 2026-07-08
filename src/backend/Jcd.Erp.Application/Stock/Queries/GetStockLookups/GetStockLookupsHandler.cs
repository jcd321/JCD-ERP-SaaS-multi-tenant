using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Application.Stock.Queries.GetStockLookups;
using Jcd.Erp.Domain.Catalog.Products;
using Jcd.Erp.Domain.Inventory.Warehouses;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Queries.GetStockLookups;

public class GetStockLookupsHandler : IRequestHandler<GetStockLookupsQuery, Result<StockLookupsResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICurrentTenantService _tenant;

    public GetStockLookupsHandler(
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        ICurrentTenantService tenant)
    {
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _tenant = tenant;
    }

    public async Task<Result<StockLookupsResult>> Handle(
        GetStockLookupsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<StockLookupsResult>("Auth.TenantRequired");

        var (products, _) = await _productRepository.GetPagedAsync(1, 500, null, true, cancellationToken);
        var (warehouses, _) = await _warehouseRepository.GetPagedAsync(1, 500, null, true, cancellationToken);

        return Result.Success(new StockLookupsResult(
            products.Select(p => new LookupOptionDto(p.Id, $"{p.Sku} — {p.Name}")).ToList(),
            warehouses.Select(w => new LookupOptionDto(w.Id, $"{w.Code} — {w.Name}")).ToList()));
    }
}
