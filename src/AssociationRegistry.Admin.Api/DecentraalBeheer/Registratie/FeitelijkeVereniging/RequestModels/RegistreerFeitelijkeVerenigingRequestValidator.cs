// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging.Exceptions;
using FluentValidation;

public class RegistreerFeitelijkeVerenigingRequestValidator : AbstractValidator<RegistreerFeitelijkeVerenigingRequest>
{
    private readonly IClock _clock;

    public RegistreerFeitelijkeVerenigingRequestValidator(IClock clock)
    {
        _clock = clock;
        this.RequireNotNullOrEmpty(request => request.Naam);

        RuleFor(request => request.Naam).MustNotContainHtml();
        RuleFor(request => request.KorteNaam).MustNotContainHtml();
        RuleFor(request => request.KorteBeschrijving).MustNotContainHtml();
        RuleForEach(request => request.HoofdactiviteitenVerenigingsloket).MustNotContainHtml();

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

        RuleFor(request => request.Werkingsgebieden)
           .SetValidator(new WerkingsgebiedenValidator());

        RuleFor(request => request.Startdatum)
           .Must(BeTodayOrBefore)
           .When(r => r.Startdatum is not null)
           .WithMessage(new StartdatumMagNietInToekomstZijn().Message);

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

    private bool BeTodayOrBefore(DateOnly? date)
        => _clock.Today >= date;

    private static bool NotHaveDuplicates(string[] values)
        => values.Length == values.DistinctBy(v => v.ToLower()).Count();
}
