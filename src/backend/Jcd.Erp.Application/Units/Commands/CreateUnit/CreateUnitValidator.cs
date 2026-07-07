using FluentValidation;

namespace Jcd.Erp.Application.Units.Commands.CreateUnit;

public class CreateUnitValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Symbol).MaximumLength(10);
    }
}
