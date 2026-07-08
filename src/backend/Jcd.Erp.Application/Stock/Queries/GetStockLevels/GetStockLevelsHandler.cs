using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Inventory.Stock;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Queries.GetStockLevels;

public class GetStockLevelsHandler : IRequestHandler<GetStockLevelsQuery, Result<PaginatedStockLevelsResult>>
{
    private readonly IStockLevelRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetStockLevelsHandler(IStockLevelRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedStockLevelsResult>> Handle(
        GetStockLevelsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedStockLevelsResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.WarehouseId,
            request.ProductId,
            request.BelowMinimumOnly,
            cancellationToken);

        var dtos = items
            .Select(s => new StockLevelDto(
                s.Id,
                s.ProductId,
                s.Product.Sku,
                s.Product.Name,
                s.Product.Unit.Symbol,
                s.WarehouseId,
                s.Warehouse.Code,
                s.Warehouse.Name,
                s.QuantityOnHand,
                s.MinQuantity,
                s.MaxQuantity,
                s.IsBelowMinimum))
            .ToList();

        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedStockLevelsResult(dtos, page, pageSize, totalCount, totalPages));
    }
}
