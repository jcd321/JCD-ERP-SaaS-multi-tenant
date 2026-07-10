using FluentValidation;

namespace Jcd.Erp.Application.Transfers.Commands.CreateInventoryTransfer;

public sealed class CreateInventoryTransferValidator : AbstractValidator<CreateInventoryTransferCommand>
{
    public CreateInventoryTransferValidator()
    {
        RuleFor(x => x.SourceWarehouseId).NotEmpty();
        RuleFor(x => x.DestinationWarehouseId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty();
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}
