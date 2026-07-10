using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.CompletePhysicalInventoryCount;

public record CompletePhysicalInventoryCountCommand(Guid CountId) : IRequest<Result>;
