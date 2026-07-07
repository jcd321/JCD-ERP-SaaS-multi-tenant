namespace Jcd.Erp.Api.Requests;

public record UpdateRoleRequest(
    string Name,
    string? Description,
    IReadOnlyList<string> PermissionCodes);
