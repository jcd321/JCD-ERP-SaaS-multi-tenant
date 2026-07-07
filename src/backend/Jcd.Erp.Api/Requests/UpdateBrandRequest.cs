namespace Jcd.Erp.Api.Requests;

public record UpdateBrandRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive);
