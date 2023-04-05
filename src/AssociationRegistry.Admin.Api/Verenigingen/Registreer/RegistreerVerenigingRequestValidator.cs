// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Linq;
using Constants;
using ContactGegevens;
using FluentValidation;
using Infrastructure.Validation;

public class RegistreerVerenigingRequestValidator : AbstractValidator<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);

        this.RequireNotNullOrEmpty(request => request.Naam);

        RuleFor(request => request.KboNummer)
            .Length(min: 10, int.MaxValue)
            .WithMessage("KboNummer moet 10 cijfers bevatten.")
            .When(request => !string.IsNullOrEmpty(request.KboNummer));

        RuleFor(request => request.Locaties)
            .Must(LocatieValidator.NotHaveDuplicates)
            .WithMessage("Identieke locaties zijn niet toegelaten.");
        RuleFor(request => request.Locaties)
            .Must(LocatieValidator.NotHaveMultipleCorresporentieLocaties)
            .WithMessage("Er mag maximum één correspondentie locatie opgegeven worden.");
        RuleFor(request => request.Locaties)
            .Must(LocatieValidator.NotHaveMultipleHoofdlocaties)
            .WithMessage("Er mag maximum één hoofdlocatie opgegeven worden.");
        RuleFor(request => request.HoofdactiviteitenVerenigingsloket)
            .Must(NotHaveDuplicates)
            .WithMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.");

        RuleForEach(request => request.Contactgegevens)
            .SetValidator(new ContactgegevenValidator());

        RuleForEach(request => request.Locaties)
            .SetValidator(new LocatieValidator());

        RuleForEach(request => request.Vertegenwoordigers)
            .SetValidator(new VertegenwoordigerValidator());
    }

    private static bool NotHaveDuplicates(string[] values)
        => values.Length == values.DistinctBy(v => v.ToLower()).Count();


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

        internal static bool NotHaveMultipleHoofdlocaties(RegistreerVerenigingRequest.Locatie[] locaties)
            => locaties.Count(l => l.Hoofdlocatie) <= 1;

        internal static bool NotHaveMultipleCorresporentieLocaties(RegistreerVerenigingRequest.Locatie[] locaties)
            => locaties.Count(l => string.Equals(l.Locatietype, Locatietypes.Correspondentie, StringComparison.InvariantCultureIgnoreCase)) <= 1;

        internal static bool NotHaveDuplicates(RegistreerVerenigingRequest.Locatie[] locaties)
            => locaties.Length == locaties.DistinctBy(ToAnonymousObject).Count();

        private static object ToAnonymousObject(RegistreerVerenigingRequest.Locatie l)
            => new { Locatietype = l.Locatietype, l.Naam, Hoofdlocatie = l.Hoofdlocatie, l.Straatnaam, l.Huisnummer, l.Busnummer, l.Postcode, l.Gemeente, l.Land };
    }

    private class VertegenwoordigerValidator : AbstractValidator<RegistreerVerenigingRequest.Vertegenwoordiger>
    {
        public VertegenwoordigerValidator()
        {
            this.RequireNotNullOrEmpty(vertegenwoordiger => vertegenwoordiger.Insz);

            RuleFor(vertegenwoordiger => vertegenwoordiger.Insz)
                .Must(ContainOnlyNumbersDotsAndDashes)
                .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Insz))
                .WithMessage("Insz heeft incorrect formaat (00.00.00-000.00 of 00000000000)");

            RuleFor(vertegenwoordiger => vertegenwoordiger.Insz)
                .Must(Have11Numbers)
                .When(
                    vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Insz) &&
                                         ContainOnlyNumbersDotsAndDashes(vertegenwoordiger.Insz))
                .WithMessage("Insz moet 11 cijfers bevatten");

            RuleForEach(vertegenwoordiger => vertegenwoordiger.Contactgegevens)
                .SetValidator(new ContactgegevenValidator());
        }

        private bool ContainOnlyNumbersDotsAndDashes(string? insz)
        {
            insz = insz!.Replace(".", string.Empty).Replace("-", string.Empty);
            return long.TryParse(insz, out _);
        }

        private bool Have11Numbers(string? insz)
        {
            insz = insz!.Replace(".", string.Empty).Replace("-", string.Empty);
            return insz.Length == 11;
        }
    }

    private class ContactgegevenValidator : AbstractValidator<RegistreerVerenigingRequest.Contactgegeven>
    {
        public ContactgegevenValidator()
        {
            this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Waarde);
            this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Type);

            RuleFor(contactgegeven => contactgegeven.Type)
                .Must(ContactgegevenType.CanParse)
                .WithMessage(contactgegeven => $"De waarde {contactgegeven.Type} is geen gekend contactgegeven type.")
                .When(contactgegeven => !string.IsNullOrEmpty(contactgegeven.Type));
        }
    }
}
