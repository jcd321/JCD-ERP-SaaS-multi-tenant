using FluentValidation;

namespace Jcd.Erp.Application.Auth.Commands.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}
