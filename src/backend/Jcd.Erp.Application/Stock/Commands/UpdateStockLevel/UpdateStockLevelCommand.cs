using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Commands.UpdateStockLevel;

public record UpdateStockLevelCommand(
    Guid Id,
    decimal QuantityOnHand,
    decimal? MinQuantity,
    decimal? MaxQuantity) : IRequest<Result>;
