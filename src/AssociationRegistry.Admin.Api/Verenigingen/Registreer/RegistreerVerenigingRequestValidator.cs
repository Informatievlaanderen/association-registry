// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Linq;
using Constants;
using Infrastructure.Validation;
using FluentValidation;

public class RegistreerVerenigingRequestValidator : AbstractValidator<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);

        this.RequireNotNullOrEmpty(request => request.Naam);

        RuleFor(request => request.KboNummer)
            .Length(10, int.MaxValue)
            .WithMessage("KboNummer moet 10 cijfers bevatten.")
            .When(request => !string.IsNullOrEmpty(request.KboNummer));

        RuleForEach(request => request.ContactInfoLijst)
            .Must(HaveAtLeastOneValue)
            .WithMessage("Een contact moet minstens één waarde bevatten.");

        RuleFor(request => request.Locaties)
            .Must(NotHaveDuplicates)
            .WithMessage("Identieke locaties zijn niet toegelaten.");
        RuleFor(request => request.Locaties)
            .Must(NotHaveMultipleCorresporentieLocaties)
            .WithMessage("Er mag maximum één coresporentie locatie opgegeven worden.");
        RuleFor(request => request.Locaties)
            .Must(NotHaveMultipleHoofdlocaties)
            .WithMessage("Er mag maximum één hoofdlocatie opgegeven worden.");

        RuleForEach(request => request.Locaties)
            .SetValidator(new LocatieValidator());

        RuleForEach(request => request.Vertegenwoordigers)
            .SetValidator(new VertegenwoordigerValidator());
    }

    private static bool NotHaveMultipleHoofdlocaties(RegistreerVerenigingRequest.Locatie[] locaties)
        => locaties.Count(l => l.Hoofdlocatie) <= 1;

    private static bool NotHaveMultipleCorresporentieLocaties(RegistreerVerenigingRequest.Locatie[] locaties)
        => locaties.Count(l => string.Equals(l.Locatietype, Locatietypes.Correspondentie, StringComparison.InvariantCultureIgnoreCase)) <= 1;

    private static bool NotHaveDuplicates(RegistreerVerenigingRequest.Locatie[] locaties)
        => locaties.Length == locaties.DistinctBy(ToAnonymousObject).Count();

    private static object ToAnonymousObject(RegistreerVerenigingRequest.Locatie l)
        => new { Locatietype = l.Locatietype, l.Naam, Hoofdlocatie = l.Hoofdlocatie, l.Straatnaam, l.Huisnummer, l.Busnummer, l.Postcode, l.Gemeente, l.Land };

    private static bool HaveAtLeastOneValue(RegistreerVerenigingRequest.ContactInfo contactInfo)
        => !string.IsNullOrEmpty(contactInfo.Email) ||
           !string.IsNullOrEmpty(contactInfo.Telefoon) ||
           !string.IsNullOrEmpty(contactInfo.Website) ||
           !string.IsNullOrEmpty(contactInfo.SocialMedia);

    private class LocatieValidator : AbstractValidator<RegistreerVerenigingRequest.Locatie>
    {
        public LocatieValidator()
        {
            this.RequireNotNullOrEmpty(locatie => locatie.Locatietype);

            RuleFor(locatie => locatie.Locatietype)
                .Must(BeAValidLocationTypeValue)
                .WithMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietypes.Correspondentie}, {Locatietypes.Activiteiten}")
                .When(locatie => !string.IsNullOrEmpty(locatie.Locatietype));

            this.RequireNotNullOrEmpty(locatie => locatie.Straatnaam);
            this.RequireNotNullOrEmpty(locatie => locatie.Huisnummer);
            this.RequireNotNullOrEmpty(locatie => locatie.Gemeente);
            this.RequireNotNullOrEmpty(locatie => locatie.Land);
            this.RequireNotNullOrEmpty(locatie => locatie.Postcode);
        }

        private static bool BeAValidLocationTypeValue(string locatieType)
            => Locatietypes.All.Contains(locatieType, StringComparer.InvariantCultureIgnoreCase);
    }

    private class VertegenwoordigerValidator: AbstractValidator<RegistreerVerenigingRequest.Vertegenwoordiger>
    {
        public VertegenwoordigerValidator()
        {
            this.RequireNotNullOrEmpty(vertegenwoordiger => vertegenwoordiger.Insz);
        }
    }

}
