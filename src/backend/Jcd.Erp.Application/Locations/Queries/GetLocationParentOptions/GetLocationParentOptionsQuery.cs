using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Locations.Queries.GetLocationParentOptions;

public record GetLocationParentOptionsQuery(
    Guid WarehouseId,
    Guid? ExcludeId = null) : IRequest<Result<IReadOnlyList<LocationParentOptionDto>>>;

public record LocationParentOptionDto(Guid Id, string Name, string Code);
