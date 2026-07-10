using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Adjustments.Commands.CreateInventoryAdjustment;

public record CreateInventoryAdjustmentLineRequest(Guid ProductId, decimal CountedQuantity);

public record CreateInventoryAdjustmentCommand(
    Guid WarehouseId,
    DateTime? AdjustmentDate,
    string Reason,
    string? Notes,
    IReadOnlyList<CreateInventoryAdjustmentLineRequest> Lines) : IRequest<Result<Guid>>;
