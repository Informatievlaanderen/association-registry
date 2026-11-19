namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Koepels.RequestModels;

using FluentValidation;
using DomainKoepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

public class WijzigKoepelgegevensRequestValidator : AbstractValidator<WijzigKoepelgegevensRequest>
{
    public const string KoepelstatusMoetGeldigZijn = "De koepelstatus moet een geldige waarde hebben.";

    public WijzigKoepelgegevensRequestValidator()
    {
        RuleFor(request => request)
            .Must(HaveAtLeastOneValue)
            .OverridePropertyName("request")
            .WithMessage("Een request mag niet leeg zijn.");

        RuleFor(request => request.Koepelstatus)
            .Must(DomainKoepelstatus.IsValid)
            .When(request => request.Koepelstatus is not null)
            .WithMessage(KoepelstatusMoetGeldigZijn);
    }

    private static bool HaveAtLeastOneValue(WijzigKoepelgegevensRequest request) =>
        request.Koepelstatus is not null || request.PrivacyVerklaringGeaccepteerd is not null;
}
