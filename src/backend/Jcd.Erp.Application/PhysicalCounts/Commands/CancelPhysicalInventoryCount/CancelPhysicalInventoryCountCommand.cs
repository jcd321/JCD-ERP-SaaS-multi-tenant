using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.CancelPhysicalInventoryCount;

public record CancelPhysicalInventoryCountCommand(Guid CountId) : IRequest<Result>;
