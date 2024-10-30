namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;

using FluentValidation;
using Infrastructure.Validation;

public class WijzigLidmaatschapRequestValidator : AbstractValidator<WijzigLidmaatschapRequest>
{
    public WijzigLidmaatschapRequestValidator()
    {
        RuleFor(r => r.Tot)
           .GreaterThanOrEqualTo(x => x.Van)
           .When(x => x.Van.HasValue)
           .WithMessage(ValidationMessages.DatumTotMoetLaterZijnDanDatumVan);

        RuleFor(r => r.Identificatie).MustNotContainHtml();
        RuleFor(r => r.Beschrijving).MustNotContainHtml();
    }
}
