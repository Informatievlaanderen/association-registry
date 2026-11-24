namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;

using FluentValidation;
using Infrastructure.WebApi.Validation;

public class BevestigRelatieaanvraagRequestValidator : AbstractValidator<BevestigRelatieaanvraagRequest>
{
    public BevestigRelatieaanvraagRequestValidator()
    {
        RuleFor(request => request.Identificatie).MustNotContainHtml();
        RuleFor(request => request.Beschrijving).MustNotContainHtml();
    }
}
