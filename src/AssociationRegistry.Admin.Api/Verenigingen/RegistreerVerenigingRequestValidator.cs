// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen;

using FluentValidation;
using Infrastructure.Validation;

public class RegistreerVerenigingRequestValidator : AbstractValidator<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequestValidator()
    {
        this.NotNullOrEmpty(request => request.Initiator);
        this.NotNullOrEmpty(request => request.Naam);
        RuleFor(request => request.KboNummer)
            .Length(10, int.MaxValue)
            .WithMessage("KboNummer moet 10 cijfers bevatten.")
            .When(request => !string.IsNullOrEmpty(request.KboNummer));
        RuleForEach(request => request.Contacten)
            .Must(HaveAtLeastOneValue)
            .WithMessage("Een contact moet minstens één waarde bevatten.");
    }

    private static bool HaveAtLeastOneValue(RegistreerVerenigingRequest.ContactInfo contactInfo)
        => !string.IsNullOrEmpty(contactInfo.Email) ||
           !string.IsNullOrEmpty(contactInfo.Telefoon) ||
           !string.IsNullOrEmpty(contactInfo.Website) ||
           !string.IsNullOrEmpty(contactInfo.SocialMedia);
}
