using FluentValidation;

namespace Jcd.Erp.Application.Settings.Commands.UpdateTenantSetting;

public class UpdateTenantSettingValidator : AbstractValidator<UpdateTenantSettingCommand>
{
    public UpdateTenantSettingValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Value).NotEmpty().MaximumLength(2000);
    }
}
