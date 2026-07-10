using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.UpdatePhysicalInventoryCountLines;

public record PhysicalInventoryCountLineUpdate(Guid LineId, decimal? CountedQuantity);

public record UpdatePhysicalInventoryCountLinesCommand(
    Guid CountId,
    IReadOnlyList<PhysicalInventoryCountLineUpdate> Lines) : IRequest<Result>;
