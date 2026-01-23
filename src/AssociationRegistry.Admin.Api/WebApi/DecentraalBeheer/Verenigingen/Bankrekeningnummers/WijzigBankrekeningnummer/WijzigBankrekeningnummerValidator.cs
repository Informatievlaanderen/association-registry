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
               .WithMessage(
                    $"{nameof(TeWijzigenBankrekeningnummer.Doel)} of {nameof(TeWijzigenBankrekeningnummer.Titularis)} moet ingevuld zijn.");

            When(x => !string.IsNullOrWhiteSpace(x.Titularis), () =>
            {
                RuleFor(x => x.Titularis)
                   .MustNotBeMoreThanAllowedMaxLength(Bankrekeningnummer.MaxLengthTitularis,
                                                      $"Titularis mag niet langer dan {Bankrekeningnummer.MaxLengthTitularis} karakters zijn.")
                   .MustNotContainHtml();
            });

            When(x => !string.IsNullOrWhiteSpace(x.Doel), () =>
            {
                RuleFor(x => x.Doel)
                   .MustNotBeMoreThanAllowedMaxLength(Bankrekeningnummer.MaxLengthDoel,
                                                      $"Doel mag niet langer dan {Bankrekeningnummer.MaxLengthDoel} karakters zijn.")
                   .MustNotContainHtml();
            });


        }

        private bool HaveDoelOrTitularis(TeWijzigenBankrekeningnummer teWijzigenBankrekeningnummer)
            => teWijzigenBankrekeningnummer.Doel is not null && teWijzigenBankrekeningnummer.Titularis is not null;
    }
}
