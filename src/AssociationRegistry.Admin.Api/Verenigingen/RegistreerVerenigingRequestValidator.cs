// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.Linq;
using Constants;
using FluentValidation;
using Infrastructure.Validation;

public class RegistreerVerenigingRequestValidator : AbstractValidator<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequestValidator()
    {
        this.NotNullOrEmpty(request => request.Initiator);
        this.NotNullOrEmpty(request => request.Naam);
        RuleFor(request => request.KboNummer)
            .Length(10, int.MaxValue)
            .WithMessage("KboNummer moet 10 cijfers bevatten.")
            .When(request => !string.IsNullOrEmpty(request.KboNummer));

        RuleFor(request => request.Locaties)
            .Must(NotHaveDuplicates)
            .WithMessage("Identieke locaties zijn niet toegelaten.");
        RuleFor(request => request.Locaties)
            .Must(NotHaveMultipleCorresporentieLocaties)
            .WithMessage("Er mag maximum één coresporentie locatie opgegeven worden.");
        RuleFor(request => request.Locaties)
            .Must(NotHaveMultipleHoofdLocaties)
            .WithMessage("Er mag maximum één hoofdlocatie opgegeven worden.");
        RuleForEach(request => request.Locaties)
            .SetValidator(new LocatieValidator());
    }

    private static bool NotHaveMultipleHoofdLocaties(RegistreerVerenigingRequest.Locatie[] locaties)
        => locaties.Count(l => l.HoofdLocatie == true) <= 1;

    private static bool NotHaveMultipleCorresporentieLocaties(RegistreerVerenigingRequest.Locatie[] locaties)
        => locaties.Count(l => string.Equals(l.LocatieType, LocatieTypes.Correspondentie, StringComparison.InvariantCultureIgnoreCase)) <= 1;

    private static bool NotHaveDuplicates(RegistreerVerenigingRequest.Locatie[] locaties)
        => locaties.Length == locaties.Distinct().Count();

    private class LocatieValidator : AbstractValidator<RegistreerVerenigingRequest.Locatie>
    {
        public LocatieValidator()
        {
            When(
                locatie => string.IsNullOrEmpty(locatie.LocatieType),
                () => { this.NotNullOrEmpty(locatie => locatie.LocatieType); }
            ).Otherwise(
                () =>
                {
                    RuleFor(locatie => locatie.LocatieType)
                        .Must(BeAValidLocationTypeValue)
                        .WithMessage($"'LocatieType' moet een geldige waarde hebben. ({LocatieTypes.Correspondentie}, {LocatieTypes.Activiteiten}");
                });

            this.NotNullOrEmpty(locatie => locatie.Straatnaam);
        }

        private static bool BeAValidLocationTypeValue(string locatieType)
            => LocatieTypes.All.Contains(locatieType, StringComparer.InvariantCultureIgnoreCase);
    }
}
