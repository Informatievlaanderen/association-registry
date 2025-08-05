namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;

using FluentValidation;
using Infrastructure.WebApi.Validation;

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
