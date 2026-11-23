namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;

using FluentValidation;
using Infrastructure.WebApi.Validation;

/// <summary>
///     Only the required fields and the beschrijving. The vCodes, the partijen (with vr-namens) and the relatieType are
///     checked by their value objects in <see cref="DienRelatieaanvraagInRequest.ToCommand" />, before anything is loaded.
/// </summary>
public class DienRelatieaanvraagInRequestValidator : AbstractValidator<DienRelatieaanvraagInRequest>
{
    public DienRelatieaanvraagInRequestValidator()
    {
        RuleFor(request => request)
            .Must(HeeftDeVerplichteVelden)
            .OverridePropertyName("request")
            .WithMessage(ValidationMessages.RelatieaanvraagVerplichteVeldenOntbreken);

        RuleFor(request => request.Beschrijving).MustNotContainHtml();
    }

    private static bool HeeftDeVerplichteVelden(DienRelatieaanvraagInRequest request) =>
        !string.IsNullOrWhiteSpace(request.VerenigingVCode)
        && !string.IsNullOrWhiteSpace(request.KoepelVCode)
        && !string.IsNullOrWhiteSpace(request.RelatieType);
}
