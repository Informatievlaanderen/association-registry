namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using System;
using System.Linq;
using Constants;
using Infrastructure.Validation;
using FluentValidation;

public class ToeTeVoegenLocatieValidator : AbstractValidator<ToeTeVoegenLocatie>
{
    public ToeTeVoegenLocatieValidator()
    {
        this.RequireNotNullOrEmpty(locatie => locatie.Locatietype);

        RuleFor(locatie => locatie.Locatietype)
            .Must(BeAValidLocationTypeValue)
            .WithMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietypes.Correspondentie}, {Locatietypes.Activiteiten}")
            .When(locatie => !string.IsNullOrEmpty(locatie.Locatietype));

        RuleFor(locatie => locatie.Adres)
            .SetValidator(new AdresValidator()!)
            .When(locatie => locatie.Adres is not null);

        RuleFor(locatie => locatie.AdresId)
            .SetValidator(new AdresIdValidator()!)
            .When(locatie => locatie.AdresId is not null);

        RuleFor(locatie => locatie)
            .Must(HaveAdresOrAdresId)
            .WithMessage("'Locatie' moet of een adres of een adresId bevatten.");
    }

    private static bool HaveAdresOrAdresId(ToeTeVoegenLocatie loc)
        => loc.AdresId is not null || loc.Adres is not null;

    private static bool BeAValidLocationTypeValue(string locatieType)
        => Locatietypes.All.Contains(locatieType, StringComparer.InvariantCultureIgnoreCase);

    internal static bool NotHaveMultipleHoofdlocaties(ToeTeVoegenLocatie[] locaties)
        => locaties.Count(l => l.Hoofdlocatie) <= 1;

    internal static bool NotHaveMultipleCorrespondentieLocaties(ToeTeVoegenLocatie[] locaties)
        => locaties.Count(l => string.Equals(l.Locatietype, Locatietypes.Correspondentie, StringComparison.InvariantCultureIgnoreCase)) <= 1;

    internal static bool NotHaveDuplicates(ToeTeVoegenLocatie[] locaties)
        => locaties.Length == locaties.DistinctBy(ToAnonymousObject).Count();

    private static object ToAnonymousObject(ToeTeVoegenLocatie l)
        => new { Locatietype = l.Locatietype, l.Naam, Hoofdlocatie = l.Hoofdlocatie, l.Adres?.Straatnaam, l.Adres?.Huisnummer, l.Adres?.Busnummer, l.Adres?.Postcode, l.Adres?.Gemeente, l.Adres?.Land, l.AdresId?.Bronwaarde, l.AdresId?.Broncode };
}
