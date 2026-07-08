namespace Jcd.Erp.Api.Requests;

public record UpdateLocationRequest(
    string Code,
    string Name,
    string? Description,
    Guid? ParentId,
    string? LocationType,
    bool IsActive);
