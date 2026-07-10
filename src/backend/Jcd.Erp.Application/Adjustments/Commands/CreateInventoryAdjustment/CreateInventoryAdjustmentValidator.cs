using FluentValidation;

namespace Jcd.Erp.Application.Adjustments.Commands.CreateInventoryAdjustment;

public sealed class CreateInventoryAdjustmentValidator : AbstractValidator<CreateInventoryAdjustmentCommand>
{
    public CreateInventoryAdjustmentValidator()
    {
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty();
            line.RuleFor(l => l.CountedQuantity).GreaterThanOrEqualTo(0);
        });
    }
}
