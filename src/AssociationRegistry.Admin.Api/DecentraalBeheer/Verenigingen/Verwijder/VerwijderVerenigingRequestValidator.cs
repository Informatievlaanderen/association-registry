namespace AssociationRegistry.Admin.Api.Verenigingen.Verwijder;

using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;

public class VerwijderVerenigingRequestValidator : AbstractValidator<VerwijderVerenigingRequest>
{
    public VerwijderVerenigingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Reden);

        RuleFor(request => request.Reden).MustNotContainHtml();
    }
}
