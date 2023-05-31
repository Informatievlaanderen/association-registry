// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;

using System;
using System.Linq;
using Constants;
using Infrastructure.Validation;
using Common;
using Vereniging;
using FluentValidation;

public class RegistreerDecentraalBeheerdeVerenigingRequestValidator : AbstractValidator<RegistreerDecentraalBeheerdeVerenigingRequest>
{
    public RegistreerDecentraalBeheerdeVerenigingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Naam);

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


    private class LocatieValidator : AbstractValidator<ToeTeVoegenLocatie>
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

        internal static bool NotHaveMultipleHoofdlocaties(ToeTeVoegenLocatie[] locaties)
            => locaties.Count(l => l.Hoofdlocatie) <= 1;

        internal static bool NotHaveMultipleCorresporentieLocaties(ToeTeVoegenLocatie[] locaties)
            => locaties.Count(l => string.Equals(l.Locatietype, Locatietypes.Correspondentie, StringComparison.InvariantCultureIgnoreCase)) <= 1;

        internal static bool NotHaveDuplicates(ToeTeVoegenLocatie[] locaties)
            => locaties.Length == locaties.DistinctBy(ToAnonymousObject).Count();

        private static object ToAnonymousObject(ToeTeVoegenLocatie l)
            => new { Locatietype = l.Locatietype, l.Naam, Hoofdlocatie = l.Hoofdlocatie, l.Straatnaam, l.Huisnummer, l.Busnummer, l.Postcode, l.Gemeente, l.Land };
    }

    private class VertegenwoordigerValidator : AbstractValidator<ToeTeVoegenVertegenwoordiger>
    {
        public VertegenwoordigerValidator()
        {
            this.RequireNotNullOrEmpty(vertegenwoordiger => vertegenwoordiger.Insz);

            this.RequireNotNullOrEmpty(vertegenwoordiger => vertegenwoordiger.Voornaam);
            RuleFor(vertegenwoordiger => vertegenwoordiger.Voornaam)
                .Must(NotContainNumbers)
                .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Voornaam))
                .WithMessage("'Voornaam' mag geen cijfers bevatten.");
            RuleFor(vertegenwoordiger => vertegenwoordiger.Voornaam)
                .Must(ContainAtLeastOneLetter)
                .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Voornaam))
                .WithMessage("'Voornaam' moet minstens een letter bevatten.");

            this.RequireNotNullOrEmpty(vertegenwoordiger => vertegenwoordiger.Achternaam);
            RuleFor(vertegenwoordiger => vertegenwoordiger.Achternaam)
                .Must(NotContainNumbers)
                .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Achternaam))
                .WithMessage("'Achternaam' mag geen cijfers bevatten.");
            RuleFor(vertegenwoordiger => vertegenwoordiger.Achternaam)
                .Must(ContainAtLeastOneLetter)
                .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Achternaam))
                .WithMessage("'Achternaam' moet minstens een letter bevatten.");

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

        private static bool NotContainNumbers(string arg)
            => !arg.Any(char.IsDigit);


        private static bool ContainAtLeastOneLetter(string arg)
            => arg.Any(char.IsLetter);
    }

    private class ContactgegevenValidator : AbstractValidator<ToeTeVoegenContactgegeven>
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
