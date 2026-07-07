using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Queries.GetCategoryParentOptions;

public record GetCategoryParentOptionsQuery(Guid? ExcludeId = null)
    : IRequest<Result<IReadOnlyList<CategoryParentOptionDto>>>;

public record CategoryParentOptionDto(Guid Id, string Name);
