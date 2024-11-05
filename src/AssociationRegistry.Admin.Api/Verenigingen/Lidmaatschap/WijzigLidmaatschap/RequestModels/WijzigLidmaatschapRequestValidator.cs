namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;

using FluentValidation;
using Infrastructure.Validation;

public class WijzigLidmaatschapRequestValidator : AbstractValidator<WijzigLidmaatschapRequest>
{
    public WijzigLidmaatschapRequestValidator()
    {
        RuleFor(r => r.Tot.Value)
           .GreaterThanOrEqualTo(x => x.Van.Value)
           .When(x => x.Van.HasValue && x.Tot.HasValue)
           .WithMessage(ValidationMessages.DatumTotMoetLaterZijnDanDatumVan);

        RuleFor(r => r.Identificatie).MustNotContainHtml();
        RuleFor(r => r.Beschrijving).MustNotContainHtml();
    }
}
