using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(
    Guid Id,
    string Code,
    string LegalName,
    string? TradeName,
    string? TaxId,
    string? Email,
    string? Phone,
    string? MobilePhone,
    string? AddressLine1,
    string? City,
    string? StateOrProvince,
    string? CountryCode,
    string? Notes,
    bool IsActive) : IRequest<Result>;
