using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Commands.DeleteLocation;

public record DeleteLocationCommand(Guid Id) : IRequest<Result>;
