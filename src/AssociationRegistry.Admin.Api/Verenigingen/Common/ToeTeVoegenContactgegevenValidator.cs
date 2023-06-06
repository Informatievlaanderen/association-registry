namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using Infrastructure.Validation;
using Vereniging;
using FluentValidation;

public class ToeTeVoegenContactgegevenValidator : AbstractValidator<ToeTeVoegenContactgegeven>
{
    public ToeTeVoegenContactgegevenValidator()
    {
        this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Waarde);
        this.RequireNotNullOrEmpty(contactgegeven => contactgegeven.Type);

        RuleFor(contactgegeven => contactgegeven.Type)
            .Must(ContactgegevenType.CanParse)
            .WithMessage(contactgegeven => $"De waarde {contactgegeven.Type} is geen gekend contactgegeven type.")
            .When(contactgegeven => !string.IsNullOrEmpty(contactgegeven.Type));
    }
}
