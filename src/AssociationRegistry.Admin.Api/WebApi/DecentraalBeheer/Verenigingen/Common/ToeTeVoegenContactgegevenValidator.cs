namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Vereniging;
using FluentValidation;

public class ToeTeVoegenContactgegevenValidator : AbstractValidator<ToeTeVoegenContactgegeven>
{
    public ToeTeVoegenContactgegevenValidator()
    {
        this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Waarde);
        this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Contactgegeventype);

        RuleFor(contactgegeven => contactgegeven.Beschrijving)
           .MustNotBeMoreThanAllowedMaxLength(Contactgegeven.MaxLengthBeschrijving,
                                              $"Beschrijving mag niet langer dan {Contactgegeven.MaxLengthBeschrijving} karakters zijn.")
           .MustNotContainHtml();

        RuleFor(contactgegeven => contactgegeven.Contactgegeventype).MustNotContainHtml();
        RuleFor(contactgegeven => contactgegeven.Waarde).MustNotContainHtml();

        RuleFor(contactgegeven => contactgegeven.Contactgegeventype)
           .Must(Contactgegeventype.CanParse)
           .WithMessage(contactgegeven => $"De waarde {contactgegeven.Contactgegeventype} is geen gekend contactgegeven type.")
           .When(contactgegeven => !string.IsNullOrEmpty(contactgegeven.Contactgegeventype));
    }
}
