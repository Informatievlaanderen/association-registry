namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe;

using Common;
using FluentValidation;
using Infrastructure.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegVertegenwoordigerToeValidator : AbstractValidator<VoegVertegenwoordigerToeRequest>
{
    public VoegVertegenwoordigerToeValidator()
    {
        RuleFor(request => request.Vertegenwoordiger).NotNull()
                                                     .WithMessage("'Vertegenwoordiger' is verplicht.");

        When(
            predicate: request => request.Vertegenwoordiger is not null,
            action: () =>
                RuleFor(request => request.Vertegenwoordiger)
                   .SetValidator(new VertegenwoordigerValidator()));
    }

    private class VertegenwoordigerValidator : AbstractValidator<ToeTeVoegenVertegenwoordiger>
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

            this.RequireNotNullOrEmpty(vertegenwoordiger => vertegenwoordiger.Voornaam);

            RuleFor(vertegenwoordiger => vertegenwoordiger.Voornaam)
               .Must(NotContainNumbers)
               .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Voornaam))
               .WithMessage("'Voornaam' mag geen cijfers bevatten.");

            RuleFor(vertegenwoordiger => vertegenwoordiger.Voornaam)
               .Must(ContainALetter)
               .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Voornaam))
               .WithMessage("'Voornaam' moet minstens een letter bevatten.");

            this.RequireNotNullOrEmpty(vertegenwoordiger => vertegenwoordiger.Achternaam);

            RuleFor(vertegenwoordiger => vertegenwoordiger.Achternaam)
               .Must(NotContainNumbers)
               .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Achternaam))
               .WithMessage("'Achternaam' mag geen cijfers bevatten.");

            RuleFor(vertegenwoordiger => vertegenwoordiger.Achternaam)
               .Must(ContainALetter)
               .When(vertegenwoordiger => !string.IsNullOrEmpty(vertegenwoordiger.Achternaam))
               .WithMessage("'Achternaam' moet minstens een letter bevatten.");
        }

        private bool ContainALetter(string arg)
            => arg.Any(char.IsLetter);

        private static bool NotContainNumbers(string arg)
            => !arg.Any(char.IsDigit);

        private static bool ContainOnlyNumbersDotsAndDashes(string? insz)
        {
            insz = insz!.Replace(oldValue: ".", string.Empty).Replace(oldValue: "-", string.Empty);

            return long.TryParse(insz, out _);
        }

        private static bool Have11Numbers(string? insz)
        {
            insz = insz!.Replace(oldValue: ".", string.Empty).Replace(oldValue: "-", string.Empty);

            return insz.Length == 11;
        }
    }
}
