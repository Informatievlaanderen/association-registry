namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;

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
