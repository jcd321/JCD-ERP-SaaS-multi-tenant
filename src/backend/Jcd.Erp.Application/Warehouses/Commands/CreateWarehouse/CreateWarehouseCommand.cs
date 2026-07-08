using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Commands.CreateWarehouse;

public record CreateWarehouseCommand(
    string Code,
    string Name,
    string? Description,
    string? AddressLine1,
    string? City,
    string? StateOrProvince,
    string? CountryCode,
    bool IsDefault) : IRequest<Result<Guid>>;
