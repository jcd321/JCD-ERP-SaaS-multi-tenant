using FluentValidation;

namespace Jcd.Erp.Application.Stock.Commands.UpdateStockLevel;

public sealed class UpdateStockLevelValidator : AbstractValidator<UpdateStockLevelCommand>
{
    public UpdateStockLevelValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.QuantityOnHand).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinQuantity).GreaterThanOrEqualTo(0).When(x => x.MinQuantity.HasValue);
        RuleFor(x => x.MaxQuantity).GreaterThanOrEqualTo(0).When(x => x.MaxQuantity.HasValue);
        RuleFor(x => x)
            .Must(x => !x.MinQuantity.HasValue || !x.MaxQuantity.HasValue || x.MinQuantity <= x.MaxQuantity)
            .WithMessage("StockLevel.MinGreaterThanMax");
    }
}
