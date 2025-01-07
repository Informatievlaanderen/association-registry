namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;
using RequestModels;

public class VerwijderVerenigingRequestValidator : AbstractValidator<VerwijderVerenigingRequest>
{
    public VerwijderVerenigingRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Reden);

        RuleFor(request => request.Reden).MustNotContainHtml();
    }
}
