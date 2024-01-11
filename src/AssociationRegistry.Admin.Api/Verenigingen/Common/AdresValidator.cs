namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Validation;

public class AdresValidator : AbstractValidator<Adres>
{
    public AdresValidator()
    {
        this.RequireNotNullOrEmpty(adres => adres.Straatnaam);
        this.RequireNotNullOrEmpty(adres => adres.Huisnummer);
        this.RequireNotNullOrEmpty(adres => adres.Gemeente);
        this.RequireNotNullOrEmpty(adres => adres.Land);
        this.RequireNotNullOrEmpty(adres => adres.Postcode);

        RuleFor(adres => adres.Straatnaam).MustNotContainHtml();
        RuleFor(adres => adres.Huisnummer).MustNotContainHtml();
        RuleFor(adres => adres.Busnummer).MustNotContainHtml();
        RuleFor(adres => adres.Postcode).MustNotContainHtml();
        RuleFor(adres => adres.Gemeente).MustNotContainHtml();
        RuleFor(adres => adres.Land).MustNotContainHtml();
    }
}
