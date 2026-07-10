using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.CreatePhysicalInventoryCount;

public record CreatePhysicalInventoryCountCommand(
    Guid WarehouseId,
    DateTime? CountDate = null,
    string? Notes = null) : IRequest<Result<Guid>>;
