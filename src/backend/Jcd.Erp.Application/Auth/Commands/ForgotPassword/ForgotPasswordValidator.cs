using FluentValidation;

namespace Jcd.Erp.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
