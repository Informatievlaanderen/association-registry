namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Subtype;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Vereniging;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigSubtypeRequestValidator : AbstractValidator<WijzigSubtypeRequest>
{
    public WijzigSubtypeRequestValidator()
    {
        RuleFor(r => r.Subtype)
           .Must(subtype => !string.IsNullOrEmpty(subtype))
           .WithVeldIsVerplichtMessage(nameof(WijzigSubtypeRequest.Subtype))
           .DependentRules(() =>
            {
                RuleFor(request => request.Subtype)
                   .Must(IsValidSubtype)
                   .WithMessage("Het subtype moet een geldige waarde hebben.");
            });

        RuleFor(r => r.Identificatie).MustNotContainHtml();
        RuleFor(r => r.Beschrijving).MustNotContainHtml();
    }

    private bool IsValidSubtype(string subtypeCode)
        => VerenigingssubtypeCode.IsValidSubtypeCode(subtypeCode);
}
