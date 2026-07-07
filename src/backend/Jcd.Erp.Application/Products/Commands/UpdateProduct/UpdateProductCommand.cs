using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Sku,
    string Name,
    Guid CategoryId,
    Guid UnitId,
    string? Description,
    Guid? BrandId,
    bool IsActive) : IRequest<Result>;
