namespace Jcd.Erp.Api.Requests;

public record UpdateUnitRequest(
    string Code,
    string Name,
    string? Symbol,
    bool IsActive);
