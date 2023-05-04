namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;

using FluentValidation;
using Infrastructure.Validation;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegVertegenwoordigerToeValidator : AbstractValidator<VoegVertegenwoordigerToeRequest>
{
    public VoegVertegenwoordigerToeValidator()
    {
        RuleFor(request => request.Vertegenwoordiger).NotNull()
            .WithMessage("'Vertegenwoordiger' is verplicht.");

        When(
            request => request.Vertegenwoordiger is not null,
            () =>
            {
                this.RequireNotNullOrEmpty(request => request.Vertegenwoordiger.Insz);

                RuleFor(request => request.Vertegenwoordiger.Insz)
                    .Must(ContainOnlyNumbersDotsAndDashes)
                    .When(request => !string.IsNullOrEmpty(request.Vertegenwoordiger.Insz))
                    .WithMessage("Insz heeft incorrect formaat (00.00.00-000.00 of 00000000000)");

                RuleFor(request => request.Vertegenwoordiger.Insz)
                    .Must(Have11Numbers)
                    .When(
                        request => !string.IsNullOrEmpty(request.Vertegenwoordiger.Insz) &&
                                   ContainOnlyNumbersDotsAndDashes(request.Vertegenwoordiger.Insz))
                    .WithMessage("Insz moet 11 cijfers bevatten");
            });
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
