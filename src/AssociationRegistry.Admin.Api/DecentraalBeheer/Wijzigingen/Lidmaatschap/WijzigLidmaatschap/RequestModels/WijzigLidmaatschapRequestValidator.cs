namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;

public class WijzigLidmaatschapRequestValidator : AbstractValidator<WijzigLidmaatschapRequest>
{
    public WijzigLidmaatschapRequestValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .OverridePropertyName("request")
           .WithMessage("Een request mag niet leeg zijn.");

        RuleFor(r => r.Tot.Value)
           .GreaterThanOrEqualTo(x => x.Van.Value)
           .When(x => x.Van.HasValue && x.Tot.HasValue)
           .OverridePropertyName(nameof(WijzigLidmaatschapRequest.Tot))
           .WithMessage(ValidationMessages.DatumTotMoetLaterZijnDanDatumVan);

        RuleFor(r => r.Identificatie).MustNotContainHtml();
        RuleFor(r => r.Beschrijving).MustNotContainHtml();
    }

    private static bool HaveAtLeastOneValue(WijzigLidmaatschapRequest request)
        => request.Beschrijving is not null ||
           request.Identificatie is not null ||
           !request.Van.IsNull ||
           !request.Tot.IsNull;
}
