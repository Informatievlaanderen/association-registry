// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;

using System.Linq;
using Infrastructure.Validation;
using Common;
using FluentValidation;

public class RegistreerFeitelijkeVerenigingRequestValidator : AbstractValidator<RegistreerFeitelijkeVerenigingRequest>
{
    public RegistreerFeitelijkeVerenigingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Naam);

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

        RuleFor(request => request.Doelgroep)
            .SetValidator(new DoelgroepRequestValidator()!)
            .When(r => r.Doelgroep is not null);

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
