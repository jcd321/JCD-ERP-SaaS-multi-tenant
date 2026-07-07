using FluentValidation;

namespace Jcd.Erp.Application.Roles.Commands.DeleteRole;

public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
