namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.RequestModels;

using FluentValidation;
using Infrastructure.WebApi.Validation;

public class TrekRelatieaanvraagInRequestValidator : AbstractValidator<TrekRelatieaanvraagInRequest>
{
    public TrekRelatieaanvraagInRequestValidator()
    {
        RuleFor(request => request.Reden).MustNotContainHtml();
    }
}
