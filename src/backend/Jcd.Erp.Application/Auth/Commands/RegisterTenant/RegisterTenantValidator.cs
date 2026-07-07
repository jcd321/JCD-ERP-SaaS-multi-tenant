using FluentValidation;

namespace Jcd.Erp.Application.Auth.Commands.RegisterTenant;

public class RegisterTenantValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Slug));
        RuleFor(x => x.AdminEmail).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.AdminPassword).NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.AdminFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AdminLastName).MaximumLength(100);
    }
}
