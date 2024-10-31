namespace AssociationRegistry.Admin.Api.Verenigingen.Subvereniging.MaakSubvereniging.RequestModels;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;

public class MaakSubverenigingRequestValidator : AbstractValidator<MaakSubverenigingRequest>
{
    public MaakSubverenigingRequestValidator()
    {
        RuleFor(r => r.AndereVereniging)
           .Must(andereVereniging => !string.IsNullOrEmpty(andereVereniging))
           .WithMessage(ValidationMessages.VeldIsVerplicht);

        RuleFor(r => r.Identificatie).MustNotContainHtml();
        RuleFor(r => r.Beschrijving).MustNotContainHtml();
    }
}
