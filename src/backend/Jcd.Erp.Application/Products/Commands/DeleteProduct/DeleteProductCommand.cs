using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;
