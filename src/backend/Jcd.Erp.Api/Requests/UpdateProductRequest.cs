namespace Jcd.Erp.Api.Requests;

public record UpdateProductRequest(
    string Sku,
    string Name,
    Guid CategoryId,
    Guid UnitId,
    string? Description,
    Guid? BrandId,
    bool IsActive);
