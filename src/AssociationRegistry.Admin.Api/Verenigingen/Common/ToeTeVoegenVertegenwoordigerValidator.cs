namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using System.Linq;
using Infrastructure.Validation;
using FluentValidation;

public class ToeTeVoegenVertegenwoordigerValidator : AbstractValidator<ToeTeVoegenVertegenwoordiger>
{
    public ToeTeVoegenVertegenwoordigerValidator()
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
