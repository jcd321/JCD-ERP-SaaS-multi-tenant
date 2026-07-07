using Jcd.Erp.Domain.Common;
using MediatR;

namespace Jcd.Erp.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentId) : IRequest<Result<Guid>>;
