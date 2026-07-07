namespace Jcd.Erp.Api.Requests;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    bool IsActive,
    IReadOnlyList<Guid> RoleIds);
