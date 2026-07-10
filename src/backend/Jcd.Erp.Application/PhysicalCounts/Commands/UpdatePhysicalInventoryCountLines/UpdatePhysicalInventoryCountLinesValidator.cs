using FluentValidation;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.UpdatePhysicalInventoryCountLines;

public class UpdatePhysicalInventoryCountLinesValidator : AbstractValidator<UpdatePhysicalInventoryCountLinesCommand>
{
    public UpdatePhysicalInventoryCountLinesValidator()
    {
        RuleFor(x => x.CountId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.LineId).NotEmpty();
            line.RuleFor(l => l.CountedQuantity).GreaterThanOrEqualTo(0).When(l => l.CountedQuantity.HasValue);
        });
    }
}
