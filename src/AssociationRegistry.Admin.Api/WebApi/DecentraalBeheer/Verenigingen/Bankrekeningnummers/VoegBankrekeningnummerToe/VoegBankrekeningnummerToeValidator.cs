namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe;

using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegBankrekeningnummerToeValidator : AbstractValidator<VoegBankrekeningnummerToeRequest>
{
    public VoegBankrekeningnummerToeValidator()
    {
        RuleFor(request => request.Bankrekeningnummer).NotNull()
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

            // TODO: add validation rules
        }
    }
}
