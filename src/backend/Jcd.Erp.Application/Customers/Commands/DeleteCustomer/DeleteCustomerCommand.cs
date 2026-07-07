using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(Guid Id) : IRequest<Result>;
