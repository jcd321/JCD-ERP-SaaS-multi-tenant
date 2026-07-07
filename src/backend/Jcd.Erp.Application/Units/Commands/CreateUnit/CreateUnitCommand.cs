using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Commands.CreateUnit;

public record CreateUnitCommand(
    string Code,
    string Name,
    string? Symbol) : IRequest<Result<Guid>>;
