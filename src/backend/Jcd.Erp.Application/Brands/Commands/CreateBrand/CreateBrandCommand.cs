using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Commands.CreateBrand;

public record CreateBrandCommand(
    string Code,
    string Name,
    string? Description) : IRequest<Result<Guid>>;
