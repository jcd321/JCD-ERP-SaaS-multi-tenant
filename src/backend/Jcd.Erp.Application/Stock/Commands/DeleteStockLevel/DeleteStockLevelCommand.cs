using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Stock.Commands.DeleteStockLevel;

public record DeleteStockLevelCommand(Guid Id) : IRequest<Result>;
