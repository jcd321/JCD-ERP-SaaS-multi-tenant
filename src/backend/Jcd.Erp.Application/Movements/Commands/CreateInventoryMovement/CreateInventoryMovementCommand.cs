using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Movements.Commands.CreateInventoryMovement;

public record CreateInventoryMovementCommand(
    Guid ProductId,
    Guid WarehouseId,
    string MovementType,
    decimal Quantity,
    DateTime? MovementDate,
    string? Reference,
    string? Notes) : IRequest<Result<Guid>>;
