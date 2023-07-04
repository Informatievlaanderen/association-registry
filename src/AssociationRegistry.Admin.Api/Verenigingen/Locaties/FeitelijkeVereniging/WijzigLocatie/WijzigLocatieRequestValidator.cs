namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;

using Common;
using FluentValidation;
using Microsoft.AspNetCore.Server.HttpSys;
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

public class TeWijzigenLocatieValidator : AbstractValidator<WijzigLocatieRequest.TeWijzigenLocatie>
{
    public TeWijzigenLocatieValidator()
    {
        RuleFor(request => request)
            .Must(HaveAtLeastOneValue)
            .WithMessage("'Locatie' moet ingevuld zijn.");

        RuleFor(locatie => locatie.Locatietype)
            .Must(BeAValidLocationTypeValue!)
            .WithMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietype.Correspondentie}, {Locatietype.Activiteiten})")
            .When(locatie => !string.IsNullOrEmpty(locatie.Locatietype));

        RuleFor(request => request.Adres)
            .SetValidator(new AdresValidator()!);

        RuleFor(request => request.AdresId)
            .SetValidator(new AdresIdValidator()!);


    }

    private bool HaveAtLeastOneValue(WijzigLocatieRequest.TeWijzigenLocatie locatie)
        => locatie.Locatietype is not null ||
           locatie.IsPrimair is not null ||
           locatie.Naam is not null ||
           locatie.Adres is not null ||
           locatie.AdresId is not null;

    private static bool BeAValidLocationTypeValue(string locatieType)
        => Locatietype.CanParse(locatieType);
}

