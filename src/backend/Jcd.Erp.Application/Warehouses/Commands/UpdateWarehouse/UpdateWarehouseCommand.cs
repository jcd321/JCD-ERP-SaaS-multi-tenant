using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Warehouses.Commands.UpdateWarehouse;

public record UpdateWarehouseCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string? AddressLine1,
    string? City,
    string? StateOrProvince,
    string? CountryCode,
    bool IsDefault,
    bool IsActive) : IRequest<Result>;
