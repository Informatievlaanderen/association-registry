namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using FluentValidation;
using Infrastructure.Validation;
using System;
using System.Linq;
using Vereniging;
using Vereniging.Exceptions;

public class ToeTeVoegenLocatieValidator : AbstractValidator<ToeTeVoegenLocatie>
{
    public const string MustHaveAdresOrAdresIdMessage = "'Locatie' moet of een adres of een adresId bevatten.";

    public ToeTeVoegenLocatieValidator()
    {
        this.RequireNotNullOrEmpty(locatie => locatie.Locatietype);

        RuleFor(locatie => locatie.Naam)
           .MustNotBeMoreThanAllowedMaxLength(Locatie.MaxLengthLocatienaam, $"Locatienaam mag niet langer dan {Locatie.MaxLengthLocatienaam} karakters zijn.")
           .MustNotContainHtml();

        RuleFor(locatie => locatie.Locatietype).MustNotContainHtml();

        RuleFor(locatie => locatie.Locatietype)
           .Must(BeAValidLocationTypeValue)
           .WithMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietype.Correspondentie}, {Locatietype.Activiteiten})")
           .When(locatie => !string.IsNullOrEmpty(locatie.Locatietype));

        RuleFor(locatie => locatie)
           .Must(HaveAdresOrAdresId)
           .WithMessage(MustHaveAdresOrAdresIdMessage);

        RuleFor(locatie => locatie.Adres)
           .SetValidator(new AdresValidator()!)
           .When(locatie => locatie.Adres is not null);

        RuleFor(locatie => locatie.AdresId)
           .SetValidator(new AdresIdValidator()!)
           .When(locatie => locatie.AdresId is not null);
    }

    private static bool HaveAdresOrAdresId(ToeTeVoegenLocatie loc)
        => (loc.AdresId is not null && loc.Adres is null) || (loc.Adres is not null && loc.AdresId is null);

    private static bool BeAValidLocationTypeValue(string locatieType)
        => Locatietype.CanParse(locatieType);

    internal static bool NotHaveMultiplePrimairelocaties(ToeTeVoegenLocatie[] locaties)
        => locaties.Count(l => l.IsPrimair) <= 1;

    internal static bool NotHaveMultipleCorrespondentieLocaties(ToeTeVoegenLocatie[] locaties)
        => locaties.Count(l => string.Equals(l.Locatietype, Locatietype.Correspondentie, StringComparison.InvariantCultureIgnoreCase)) <= 1;

    internal static bool NotHaveDuplicates(ToeTeVoegenLocatie[] locaties)
        => locaties.Length == locaties.DistinctBy(ToAnonymousObject).Count();

    private static object ToAnonymousObject(ToeTeVoegenLocatie l)
        => new
        {
            l.Locatietype, l.Naam, l.Adres?.Straatnaam, l.Adres?.Huisnummer, l.Adres?.Busnummer, l.Adres?.Postcode, l.Adres?.Gemeente,
            l.Adres?.Land, l.AdresId?.Bronwaarde, l.AdresId?.Broncode,
        };
}
