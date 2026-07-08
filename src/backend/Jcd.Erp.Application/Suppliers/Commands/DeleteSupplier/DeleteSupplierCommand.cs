using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Suppliers.Commands.DeleteSupplier;

public record DeleteSupplierCommand(Guid Id) : IRequest<Result>;
