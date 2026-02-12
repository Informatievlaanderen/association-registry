namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe;

using DecentraalBeheer.Vereniging.Bankrekeningen;
using DecentraalBeheer.Vereniging.Bankrekeningen.IbanBic;
using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;

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
            this.RequireNotNullOrEmpty(Bankrekeningnummer => Bankrekeningnummer.Titularis);

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
                x => !string.IsNullOrWhiteSpace(x.Titularis),
                () =>
                {
                    RuleFor(x => x.Titularis)
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
