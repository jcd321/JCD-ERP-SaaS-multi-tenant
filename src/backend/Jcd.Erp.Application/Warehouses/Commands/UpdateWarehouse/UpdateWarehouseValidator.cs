using FluentValidation;

namespace Jcd.Erp.Application.Warehouses.Commands.UpdateWarehouse;

public sealed class UpdateWarehouseValidator : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.AddressLine1).MaximumLength(200);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.StateOrProvince).MaximumLength(100);
        RuleFor(x => x.CountryCode).MaximumLength(2);
    }
}
