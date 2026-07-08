using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Commands.CreateStockLevel;

public record CreateStockLevelCommand(
    Guid ProductId,
    Guid WarehouseId,
    decimal QuantityOnHand,
    decimal? MinQuantity,
    decimal? MaxQuantity) : IRequest<Result<Guid>>;
