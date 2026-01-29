namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentValidation;
using RequestModels;
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
            action: () =>
                RuleFor(request => request.Bankrekeningnummer)
                   .SetValidator(new BankrekeningnummerValidator()));
    }

    private class BankrekeningnummerValidator : AbstractValidator<TeWijzigenBankrekeningnummer>
    {
        public BankrekeningnummerValidator()
        {
            RuleFor(x => x)
               .Must(HaveDoelOrTitularis)
               .WithMessage($"{nameof(TeWijzigenBankrekeningnummer.Doel)} of {nameof(TeWijzigenBankrekeningnummer.Titularis)} moet ingevuld zijn.");

            When(x => x.Titularis is not null, () =>
            {
                RuleFor(x => x.Titularis)
                   .Must(t => !string.IsNullOrWhiteSpace(t))
                   .WithMessage($"{nameof(TeWijzigenBankrekeningnummer.Titularis)} mag niet leeg zijn.")
                   .MustNotBeMoreThanAllowedMaxLength(
                        Bankrekeningnummer.MaxLengthTitularis,
                        $"Titularis mag niet langer dan {Bankrekeningnummer.MaxLengthTitularis} karakters zijn.")
                   .MustNotContainHtml();
            });

            When(x => x.Doel is not null, () =>
            {
                RuleFor(x => x.Doel)
                   .MustNotBeMoreThanAllowedMaxLength(
                        Bankrekeningnummer.MaxLengthDoel,
                        $"Doel mag niet langer dan {Bankrekeningnummer.MaxLengthDoel} karakters zijn.")
                   .MustNotContainHtml();
            });
        }

        private static bool HaveDoelOrTitularis(TeWijzigenBankrekeningnummer x)
            => !(x.Doel is null && x.Titularis is null);
    }
}
