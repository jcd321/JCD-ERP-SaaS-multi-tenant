using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Dashboard.Queries.GetDashboardKpis;

public record GetDashboardKpisQuery : IRequest<Result<DashboardKpisDto>>;

public record DashboardKpisDto(
    int ProductsCount,
    int StockRecordsCount,
    decimal TotalQuantityOnHand,
    int BelowMinimumCount,
    int WarehousesCount,
    int? OrdersCount,
    decimal? Revenue);
