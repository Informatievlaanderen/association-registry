namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using FluentValidation;

public class TeWijzigenVertegenwoordigerValidator : AbstractValidator<TeWijzigenVertegenwoordiger>
{
    public TeWijzigenVertegenwoordigerValidator()
    {
        RuleFor(vertegenwoordiger => vertegenwoordiger.Email).MustNotContainHtml();
        RuleFor(vertegenwoordiger => vertegenwoordiger.Mobiel).MustNotContainHtml();
        RuleFor(vertegenwoordiger => vertegenwoordiger.SocialMedia).MustNotContainHtml();
        RuleFor(vertegenwoordiger => vertegenwoordiger.Roepnaam).MustNotContainHtml();
        RuleFor(vertegenwoordiger => vertegenwoordiger.Rol).MustNotContainHtml();
    }
}
