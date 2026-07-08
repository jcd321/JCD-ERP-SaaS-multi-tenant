using FluentValidation;

namespace Jcd.Erp.Application.Stock.Commands.CreateStockLevel;

public sealed class CreateStockLevelValidator : AbstractValidator<CreateStockLevelCommand>
{
    public CreateStockLevelValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.QuantityOnHand).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinQuantity).GreaterThanOrEqualTo(0).When(x => x.MinQuantity.HasValue);
        RuleFor(x => x.MaxQuantity).GreaterThanOrEqualTo(0).When(x => x.MaxQuantity.HasValue);
        RuleFor(x => x)
            .Must(x => !x.MinQuantity.HasValue || !x.MaxQuantity.HasValue || x.MinQuantity <= x.MaxQuantity)
            .WithMessage("StockLevel.MinGreaterThanMax");
    }
}
