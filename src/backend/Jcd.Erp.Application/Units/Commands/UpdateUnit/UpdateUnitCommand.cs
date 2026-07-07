using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Commands.UpdateUnit;

public record UpdateUnitCommand(
    Guid Id,
    string Code,
    string Name,
    string? Symbol,
    bool IsActive) : IRequest<Result>;
