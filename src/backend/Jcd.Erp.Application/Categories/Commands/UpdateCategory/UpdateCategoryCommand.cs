using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentId,
    bool IsActive) : IRequest<Result>;
