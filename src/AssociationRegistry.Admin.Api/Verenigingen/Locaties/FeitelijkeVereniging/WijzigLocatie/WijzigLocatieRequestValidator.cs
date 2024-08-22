namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;

using Common;
using FluentValidation;
using Infrastructure.Validation;
using RequestModels;
using Vereniging;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigLocatieRequestValidator : AbstractValidator<WijzigLocatieRequest>
{
    public WijzigLocatieRequestValidator()
    {
        RuleFor(request => request.Locatie)
           .NotNull()
           .WithMessage("'Locatie' is verplicht.");

        RuleFor(request => request.Locatie)
           .SetValidator(new TeWijzigenLocatieValidator());
    }
}

public class TeWijzigenLocatieValidator : AbstractValidator<TeWijzigenLocatie>
{
    public const string MustHaveAdresOrAdresIdMessage = "'Locatie' moet of een adres of een adresId bevatten.";

    public TeWijzigenLocatieValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .WithMessage("'Locatie' moet ingevuld zijn.");

        RuleFor(locatie => locatie.Naam)
           .MustNotBeMoreThanAllowedMaxLength(Locatie.MaxLengthLocatienaam,
                                              $"Locatienaam mag niet langer dan {Locatie.MaxLengthLocatienaam} karakters zijn.");

        RuleFor(locatie => locatie.Locatietype)
           .Must(BeAValidLocationTypeValue!)
           .WithMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietype.Correspondentie}, {Locatietype.Activiteiten})")
           .When(locatie => locatie.Locatietype is not null);

        RuleFor(locatie => locatie)
           .Must(HaveAdresOrAdresIdOrBothBeNull)
           .WithMessage(MustHaveAdresOrAdresIdMessage);

        RuleFor(locatie => locatie.Adres)
           .SetValidator(new AdresValidator()!)
           .When(locatie => locatie.Adres is not null);

        RuleFor(locatie => locatie.AdresId)
           .SetValidator(new AdresIdValidator()!)
           .When(locatie => locatie.AdresId is not null);
    }

    private static bool HaveAdresOrAdresIdOrBothBeNull(TeWijzigenLocatie loc)
        => (loc.AdresId is not null && loc.Adres is null) || (loc.Adres is not null && loc.AdresId is null)
                                                          || (loc.AdresId is null && loc.Adres is null);

    private bool HaveAtLeastOneValue(TeWijzigenLocatie locatie)
        => locatie.Locatietype is not null ||
           locatie.IsPrimair is not null ||
           locatie.Naam is not null ||
           locatie.Adres is not null ||
           locatie.AdresId is not null;

    private static bool BeAValidLocationTypeValue(string locatieType)
        => Locatietype.CanParse(locatieType);
}
