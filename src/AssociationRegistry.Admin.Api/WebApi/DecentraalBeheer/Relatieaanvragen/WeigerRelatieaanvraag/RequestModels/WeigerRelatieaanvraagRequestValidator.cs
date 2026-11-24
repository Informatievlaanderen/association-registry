namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;

using FluentValidation;
using Infrastructure.WebApi.Validation;

public class WeigerRelatieaanvraagRequestValidator : AbstractValidator<WeigerRelatieaanvraagRequest>
{
    public WeigerRelatieaanvraagRequestValidator()
    {
        RuleFor(request => request.Reden).MustNotContainHtml();
    }
}
