using FluentValidation;

namespace Jcd.Erp.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.LegalName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TradeName).MaximumLength(200);
        RuleFor(x => x.TaxId).MaximumLength(30);
        RuleFor(x => x.Email).MaximumLength(200).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.MobilePhone).MaximumLength(30);
        RuleFor(x => x.AddressLine1).MaximumLength(300);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.StateOrProvince).MaximumLength(100);
        RuleFor(x => x.CountryCode).MaximumLength(2);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
