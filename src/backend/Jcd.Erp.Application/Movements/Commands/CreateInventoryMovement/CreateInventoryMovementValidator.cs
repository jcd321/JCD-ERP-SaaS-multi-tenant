using FluentValidation;

namespace Jcd.Erp.Application.Movements.Commands.CreateInventoryMovement;

public sealed class CreateInventoryMovementValidator : AbstractValidator<CreateInventoryMovementCommand>
{
    public CreateInventoryMovementValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.MovementType).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Reference).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
