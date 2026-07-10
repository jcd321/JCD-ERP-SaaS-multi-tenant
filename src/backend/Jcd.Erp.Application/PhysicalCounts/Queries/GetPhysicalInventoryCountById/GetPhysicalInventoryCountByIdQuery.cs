using Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCounts;
using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.PhysicalCounts.Queries.GetPhysicalInventoryCountById;

public record GetPhysicalInventoryCountByIdQuery(Guid Id) : IRequest<Result<PhysicalInventoryCountDto>>;
