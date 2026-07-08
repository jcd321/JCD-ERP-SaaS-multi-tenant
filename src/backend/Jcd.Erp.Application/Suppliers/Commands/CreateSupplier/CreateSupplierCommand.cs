using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(
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
    string? Notes) : IRequest<Result<Guid>>;
