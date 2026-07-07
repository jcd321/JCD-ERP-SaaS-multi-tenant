using FluentValidation;

namespace Jcd.Erp.Application.Units.Commands.UpdateUnit;

public class UpdateUnitValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Symbol).MaximumLength(10);
    }
}
