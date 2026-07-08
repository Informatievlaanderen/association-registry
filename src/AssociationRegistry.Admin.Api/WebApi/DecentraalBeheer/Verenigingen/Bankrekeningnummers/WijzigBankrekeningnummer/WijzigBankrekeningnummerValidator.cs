namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.Extensions;
using FluentValidation;
using RequestModels;
using Resources;
using TeWijzigenBankrekeningnummer = RequestModels.TeWijzigenBankrekeningnummer;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigBankrekeningnummerValidator : AbstractValidator<WijzigBankrekeningnummerRequest>
{
    public WijzigBankrekeningnummerValidator()
    {
        RuleFor(request => request.Bankrekeningnummer)
            .NotNull()
            .WithVeldIsVerplichtMessage(nameof(WijzigBankrekeningnummerRequest.Bankrekeningnummer));

        When(
            predicate: request => request.Bankrekeningnummer is not null,
            action: () => RuleFor(request => request.Bankrekeningnummer).SetValidator(new BankrekeningnummerValidator())
        );
    }

    private class BankrekeningnummerValidator : AbstractValidator<TeWijzigenBankrekeningnummer>
    {
        public BankrekeningnummerValidator()
        {
            RuleFor(x => x)
                .Must(HaveDoelOrTitularis)
                .WithMessage(
                    $"{nameof(TeWijzigenBankrekeningnummer.Doel)} of {nameof(TeWijzigenBankrekeningnummer.Titularissen)} moet ingevuld zijn."
                );

            When(
                x => x.Titularissen is not null,
                () =>
                {
                    RuleFor(x => x.Titularissen)
                        .Must(t => !t.HasCaseInsensitiveDuplicateValues())
                        .When(x => !x.Titularissen.HasNullOrWhiteSpaceValues(), ApplyConditionTo.CurrentValidator)
                        .WithMessage(ExceptionMessages.TitularisMoetUniekZijn);

                    RuleFor(x => x.Titularissen)
                        .NotEmpty()
                        .When(x => x.Doel is not null)
                        .WithMessage($"'{nameof(TeWijzigenBankrekeningnummer.Titularissen)}' mag niet leeg zijn.");

                    RuleForEach(x => x.Titularissen)
                        .Cascade(CascadeMode.Stop) // <-- zonder dit: NRE in max-length check bij null element
                        .Must(t => !string.IsNullOrWhiteSpace(t))
                        .WithMessage($"{nameof(TeWijzigenBankrekeningnummer.Titularissen)} mag niet leeg zijn.")
                        .MustNotBeMoreThanAllowedMaxLength(
                            Bankrekeningnummer.MaxLengthTitularis,
                            $"Titularis mag niet langer dan {Bankrekeningnummer.MaxLengthTitularis} karakters zijn."
                        )
                        .MustNotContainHtml();
                }
            );

            When(
                x => x.Doel is not null,
                () =>
                {
                    RuleFor(x => x.Doel)
                        .MustNotBeMoreThanAllowedMaxLength(
                            Bankrekeningnummer.MaxLengthDoel,
                            $"Doel mag niet langer dan {Bankrekeningnummer.MaxLengthDoel} karakters zijn."
                        )
                        .MustNotContainHtml();
                }
            );
        }

        private static bool HaveDoelOrTitularis(TeWijzigenBankrekeningnummer x) =>
            x.Doel is not null || x.Titularissen?.Length > 0;
    }
}
