namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;

public class VoegLidmaatschapToeRequestValidator : AbstractValidator<VoegLidmaatschapToeRequest>
{
    public VoegLidmaatschapToeRequestValidator()
    {
        RuleFor(r => r.AndereVereniging)
           .Must(andereVereniging => !string.IsNullOrEmpty(andereVereniging))
           .WithVeldIsVerplichtMessage(nameof(VoegLidmaatschapToeRequest.AndereVereniging));

        RuleFor(r => r.Tot)
           .GreaterThanOrEqualTo(x => x.Van)
           .When(x => x.Van.HasValue)
           .WithMessage(ValidationMessages.DatumTotMoetLaterZijnDanDatumVan);

        RuleFor(r => r.Identificatie).MustNotContainHtml();
        RuleFor(r => r.Beschrijving).MustNotContainHtml();
    }
}
