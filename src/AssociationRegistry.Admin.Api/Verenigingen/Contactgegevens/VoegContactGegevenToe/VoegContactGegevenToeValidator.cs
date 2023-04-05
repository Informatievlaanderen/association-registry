namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using AssociationRegistry.ContactGegevens;
using FluentValidation;

public class VoegContactgegevenToeValidator : AbstractValidator<VoegContactgegevenToeRequest>
{
    public VoegContactgegevenToeValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);

        RuleFor(request => request.Contactgegeven).NotNull()
            .WithMessage("'Contactgegeven' is verplicht.");
        When(request => request.Contactgegeven is not null,
            () => this.RequireNotNullOrEmpty(request => request.Contactgegeven.Waarde)
        );
        When(request => request.Contactgegeven is not null,
            () => RuleFor(request => request.Contactgegeven.Type)
                .Must(ContactgegevenType.CanParse)
                .WithMessage("'Type' is verplicht.")
        );
    }
}
