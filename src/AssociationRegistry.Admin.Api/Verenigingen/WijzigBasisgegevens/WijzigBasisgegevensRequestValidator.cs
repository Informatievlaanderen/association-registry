// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using CommonRequestDataTypes;
using FluentValidation;
using Infrastructure.Validation;

public class WijzigBasisgegevensRequestValidator : AbstractValidator<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);

        RuleFor(request => request)
            .Must(HaveAtLeastOneValue)
            .OverridePropertyName("request")
            .WithMessage("Een request mag niet leeg zijn.");
        RuleFor(request => request.Naam)
            .Must(naam => naam?.Trim() is null or not "")
            .WithMessage("'Naam' mag niet leeg zijn.");

        When(
            request => request.ContactInfoLijst is not null,
            () =>
            {
                RuleFor(request => request.ContactInfoLijst)
                    .Must(ContactInfoValidator.NotHaveDuplicateContactnaam!)
                    .WithMessage("Een contactnaam moet uniek zijn.");
                RuleFor(request => request.ContactInfoLijst)
                    .Must(ContactInfoValidator.NotHaveMultiplePrimaryContactInfos!)
                    .WithMessage("Er mag maximum één primair contactinfo opgegeven worden.");

                RuleForEach(request => request.ContactInfoLijst)
                    .SetValidator(new ContactInfoValidator());
            });
    }

    private static bool HaveAtLeastOneValue(WijzigBasisgegevensRequest request)
        => request.Naam is not null ||
           request.KorteNaam is not null ||
           request.KorteBeschrijving is not null ||
           !request.Startdatum.IsNull ||
           request.ContactInfoLijst is not null;
}
