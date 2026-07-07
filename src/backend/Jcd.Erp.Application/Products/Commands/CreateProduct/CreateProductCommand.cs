using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Sku,
    string Name,
    Guid CategoryId,
    Guid UnitId,
    string? Description = null,
    Guid? BrandId = null) : IRequest<Result<Guid>>;
