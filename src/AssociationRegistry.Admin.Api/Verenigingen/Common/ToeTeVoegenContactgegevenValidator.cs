namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using FluentValidation;
using Infrastructure.Validation;
using Vereniging;

public class ToeTeVoegenContactgegevenValidator : AbstractValidator<ToeTeVoegenContactgegeven>
{
    public ToeTeVoegenContactgegevenValidator()
    {
        this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Waarde);
        this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Contactgegeventype);

        RuleFor(contactgegeven => contactgegeven.Beschrijving).MustNotContainHtml();
        RuleFor(contactgegeven => contactgegeven.Contactgegeventype).MustNotContainHtml();
        RuleFor(contactgegeven => contactgegeven.Waarde).MustNotContainHtml();

        RuleFor(contactgegeven => contactgegeven.Contactgegeventype)
           .Must(Contactgegeventype.CanParse)
           .WithMessage(contactgegeven => $"De waarde {contactgegeven.Contactgegeventype} is geen gekend contactgegeven type.")
           .When(contactgegeven => !string.IsNullOrEmpty(contactgegeven.Contactgegeventype));
    }
}
