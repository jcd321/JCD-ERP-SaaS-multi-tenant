using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Commands.UpdateBrand;

public record UpdateBrandCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive) : IRequest<Result>;
