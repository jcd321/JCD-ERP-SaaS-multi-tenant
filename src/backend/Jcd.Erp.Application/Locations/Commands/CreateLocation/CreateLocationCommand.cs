using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Commands.CreateLocation;

public record CreateLocationCommand(
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    Guid? ParentId,
    string? LocationType) : IRequest<Result<Guid>>;
