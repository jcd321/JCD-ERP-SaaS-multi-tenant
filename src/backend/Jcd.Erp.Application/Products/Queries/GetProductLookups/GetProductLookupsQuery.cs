using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Queries.GetProductLookups;

public record GetProductLookupsQuery : IRequest<Result<ProductLookupsResult>>;

public record LookupOptionDto(Guid Id, string Name);

public record ProductLookupsResult(
    IReadOnlyList<LookupOptionDto> Categories,
    IReadOnlyList<LookupOptionDto> Brands,
    IReadOnlyList<LookupOptionDto> Units);
