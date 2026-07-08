namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe;

using AssociationRegistry.Extensions;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using DecentraalBeheer.Vereniging.Bankrekeningen.IbanBic;
using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;
using Resources;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegBankrekeningnummerToeValidator : AbstractValidator<VoegBankrekeningnummerToeRequest>
{
    public VoegBankrekeningnummerToeValidator()
    {
        RuleFor(request => request.Bankrekeningnummer)
            .NotNull()
            .WithVeldIsVerplichtMessage(nameof(VoegBankrekeningnummerToeRequest.Bankrekeningnummer));

        When(
            predicate: request => request.Bankrekeningnummer is not null,
            action: () => RuleFor(request => request.Bankrekeningnummer).SetValidator(new BankrekeningnummerValidator())
        );
    }

    public class BankrekeningnummerValidator : AbstractValidator<ToeTeVoegenBankrekeningnummer>
    {
        public BankrekeningnummerValidator()
        {
            this.RequireNotNullOrEmpty(Bankrekeningnummer => Bankrekeningnummer.Iban);

            RuleFor(x => x.Titularissen)
                .Cascade(CascadeMode.Stop) // avoid double error when null (NotNull + NotEmpty both fire otherwise)
                .NotNull()
                .WithVeldIsVerplichtMessage(nameof(ToeTeVoegenBankrekeningnummer.Titularissen))
                .NotEmpty()
                .WithMessage($"'{nameof(ToeTeVoegenBankrekeningnummer.Titularissen)}' mag niet leeg zijn.");

            When(
                x => !string.IsNullOrWhiteSpace(x.Iban),
                () =>
                {
                    RuleFor(x => x.Iban)
                        .Must(IbanNummer.IsValid)
                        .WithMessage("Het opgegeven 'IBAN' is geen geldig Belgisch IBAN.");
                }
            );

            When(
                x => x.Titularissen != null && x.Titularissen.Any(),
                () =>
                {
                    RuleFor(x => x.Titularissen)
                        .Must(t => !t.HasCaseInsensitiveDuplicateValues())
                        .When(x => !x.Titularissen.HasNullOrWhiteSpaceValues(), ApplyConditionTo.CurrentValidator)
                        .WithMessage(ExceptionMessages.TitularisMoetUniekZijn);

                    RuleForEach(x => x.Titularissen)
                        .Cascade(CascadeMode.Stop) // null element: stop before max-length check (would NRE)
                        .NotNull()
                        .WithVeldIsVerplichtMessage(nameof(ToeTeVoegenBankrekeningnummer.Titularissen))
                        .NotEmpty()
                        .WithMessage($"'{nameof(ToeTeVoegenBankrekeningnummer.Titularissen)}' mag niet leeg zijn.")
                        .MustNotBeMoreThanAllowedMaxLength(
                            Bankrekeningnummer.MaxLengthTitularis,
                            $"Titularis mag niet langer dan {Bankrekeningnummer.MaxLengthTitularis} karakters zijn."
                        )
                        .MustNotContainHtml();
                }
            );

            When(
                x => !string.IsNullOrWhiteSpace(x.Doel),
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
    }
}
