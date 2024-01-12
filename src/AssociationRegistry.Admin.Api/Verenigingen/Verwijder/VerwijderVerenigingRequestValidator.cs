namespace AssociationRegistry.Admin.Api.Verenigingen.Verwijderen;

using FluentValidation;
using Infrastructure.Validation;
using RequestModels;

public class VerwijderVerenigingRequestValidator : AbstractValidator<VerwijderVerenigingRequest>
{
    public VerwijderVerenigingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Reden);

        RuleFor(request => request.Reden).MustNotContainHtml();
    }
}
