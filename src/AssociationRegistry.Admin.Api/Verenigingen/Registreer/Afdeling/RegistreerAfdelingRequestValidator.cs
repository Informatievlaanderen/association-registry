// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;

using System.Linq;
using Common;
using Infrastructure.Validation;
using FluentValidation;

public class RegistreerAfdelingRequestValidator : AbstractValidator<RegistreerAfdelingRequest>
{
    public RegistreerAfdelingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Naam);

        this.RequireValidKboNummer(request => request.KboNummerMoedervereniging);

        RuleFor(request => request.Locaties)
            .Must(ToeTeVoegenLocatieValidator.NotHaveDuplicates)
            .WithMessage("Identieke locaties zijn niet toegelaten.");
        RuleFor(request => request.Locaties)
            .Must(ToeTeVoegenLocatieValidator.NotHaveMultipleCorrespondentieLocaties)
            .WithMessage("Er mag maximum één correspondentie locatie opgegeven worden.");
        RuleFor(request => request.Locaties)
            .Must(ToeTeVoegenLocatieValidator.NotHaveMultiplePrimairelocaties)
            .WithMessage("Er mag maximum één primaire locatie opgegeven worden.");
        RuleFor(request => request.HoofdactiviteitenVerenigingsloket)
            .Must(NotHaveDuplicates)
            .WithMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.");

        RuleForEach(request => request.Contactgegevens)
            .SetValidator(new ToeTeVoegenContactgegevenValidator());

        RuleForEach(request => request.Locaties)
            .SetValidator(new ToeTeVoegenLocatieValidator());

        RuleForEach(request => request.Vertegenwoordigers)
            .SetValidator(new ToeTeVoegenVertegenwoordigerValidator());
    }

    private static bool NotHaveDuplicates(string[] values)
        => values.Length == values.DistinctBy(v => v.ToLower()).Count();
}
