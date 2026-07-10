using Jcd.Erp.Application.Common.Interfaces;
using Jcd.Erp.Domain.Common;
using Jcd.Erp.Domain.Inventory.Transfers;
using MediatR;

namespace Jcd.Erp.Application.Transfers.Queries.GetInventoryTransfers;

public class GetInventoryTransfersHandler : IRequestHandler<GetInventoryTransfersQuery, Result<PaginatedInventoryTransfersResult>>
{
    private readonly IInventoryTransferRepository _repository;
    private readonly ICurrentTenantService _tenant;

    public GetInventoryTransfersHandler(IInventoryTransferRepository repository, ICurrentTenantService tenant)
    {
        _repository = repository;
        _tenant = tenant;
    }

    public async Task<Result<PaginatedInventoryTransfersResult>> Handle(
        GetInventoryTransfersQuery request,
        CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant)
            return Result.Failure<PaginatedInventoryTransfersResult>("Auth.TenantRequired");

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            request.Search,
            request.SourceWarehouseId,
            request.DestinationWarehouseId,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return Result.Success(new PaginatedInventoryTransfersResult(dtos, page, pageSize, totalCount, totalPages));
    }

    private static InventoryTransferDto MapToDto(InventoryTransfer transfer)
    {
        var lines = transfer.Lines
            .OrderBy(l => l.LineNumber)
            .Select(l => new InventoryTransferLineDto(
                l.Id,
                l.ProductId,
                l.Product.Sku,
                l.Product.Name,
                l.Product.Unit.Symbol,
                l.Quantity,
                l.LineNumber))
            .ToList();

        return new InventoryTransferDto(
            transfer.Id,
            transfer.DocumentNumber,
            transfer.SourceWarehouseId,
            transfer.SourceWarehouse.Code,
            transfer.SourceWarehouse.Name,
            transfer.DestinationWarehouseId,
            transfer.DestinationWarehouse.Code,
            transfer.DestinationWarehouse.Name,
            transfer.TransferDate,
            transfer.Notes,
            lines.Count,
            lines,
            transfer.CreatedAt);
    }
}
