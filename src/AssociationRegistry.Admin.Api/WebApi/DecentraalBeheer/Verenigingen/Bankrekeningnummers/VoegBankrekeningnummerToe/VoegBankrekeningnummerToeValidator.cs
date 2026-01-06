namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe;

using DecentraalBeheer.Vereniging.Bankrekeningen.IbanBic;
using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;
using System.Text.RegularExpressions;

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
            action: () =>
                RuleFor(request => request.Bankrekeningnummer)
                   .SetValidator(new BankrekeningnummerValidator()));
    }

    private class BankrekeningnummerValidator : AbstractValidator<ToeTeVoegenBankrekeningnummer>
    {
        public BankrekeningnummerValidator()
        {
            this.RequireNotNullOrEmpty(Bankrekeningnummer => Bankrekeningnummer.IBAN);

            When(x => !string.IsNullOrWhiteSpace(x.IBAN), () =>
            {
                RuleFor(x => x.IBAN)
                   .Matches(@"^BE\d{14}$")
                   .WithMessage("Het opgegeven 'IBAN' is geen geldig Belgisch IBAN.");

                RuleFor(x => x.IBAN)
                   .Must(x => IbanUtils.IsValid(x, out _))
                   .WithMessage("Het opgegeven 'IBAN' is geen geldig Belgisch IBAN.");
            });
        }
    }
}
