using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Commands.DeleteWarehouse;

public record DeleteWarehouseCommand(Guid Id) : IRequest<Result>;
