using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Units.Commands.DeleteUnit;

public record DeleteUnitCommand(Guid Id) : IRequest<Result>;
