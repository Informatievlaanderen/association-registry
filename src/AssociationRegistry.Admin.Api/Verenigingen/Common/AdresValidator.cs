namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using FluentValidation;
using Infrastructure.Validation;

public class AdresValidator : AbstractValidator<ToeTeVoegenAdres>
{
    public AdresValidator()
    {
        this.RequireNotNullOrEmpty(adres => adres.Straatnaam);
        this.RequireNotNullOrEmpty(locatie => locatie.Huisnummer);
        this.RequireNotNullOrEmpty(locatie => locatie.Gemeente);
        this.RequireNotNullOrEmpty(locatie => locatie.Land);
        this.RequireNotNullOrEmpty(locatie => locatie.Postcode);
    }
}
