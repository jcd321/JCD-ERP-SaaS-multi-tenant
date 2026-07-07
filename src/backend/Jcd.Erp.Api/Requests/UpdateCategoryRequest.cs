namespace Jcd.Erp.Api.Requests;

public record UpdateCategoryRequest(
    string Name,
    string? Description,
    Guid? ParentId,
    bool IsActive);
