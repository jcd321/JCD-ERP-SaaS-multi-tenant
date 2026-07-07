using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Brands.Commands.DeleteBrand;

public record DeleteBrandCommand(Guid Id) : IRequest<Result>;
