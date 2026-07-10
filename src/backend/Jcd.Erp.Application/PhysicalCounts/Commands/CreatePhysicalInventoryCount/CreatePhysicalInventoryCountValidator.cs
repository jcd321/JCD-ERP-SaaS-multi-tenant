using FluentValidation;

namespace Jcd.Erp.Application.PhysicalCounts.Commands.CreatePhysicalInventoryCount;

public class CreatePhysicalInventoryCountValidator : AbstractValidator<CreatePhysicalInventoryCountCommand>
{
    public CreatePhysicalInventoryCountValidator()
    {
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}
