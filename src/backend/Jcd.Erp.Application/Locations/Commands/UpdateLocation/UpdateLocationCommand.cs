using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Commands.UpdateLocation;

public record UpdateLocationCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    Guid? ParentId,
    string? LocationType,
    bool IsActive) : IRequest<Result>;
