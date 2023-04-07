namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;

using Infrastructure.Validation;
using ContactGegevens;
using FluentValidation;

public class VoegContactgegevenToeValidator : AbstractValidator<VoegContactgegevenToeRequest>
{
    public VoegContactgegevenToeValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);

        RuleFor(request => request.Contactgegeven).NotNull()
            .WithMessage("'Contactgegeven' is verplicht.");
        When(
            request => request.Contactgegeven is not null,
            () => this.RequireNotNullOrEmpty(request => request.Contactgegeven.Waarde)
        );
        When(
            request => request.Contactgegeven is not null,
            () => this.RequireNotNullOrEmpty(request => request.Contactgegeven.Type)
        );
        When(
            request => request.Contactgegeven is not null && !string.IsNullOrEmpty(request.Contactgegeven.Type),
            () => RuleFor(request => request.Contactgegeven.Type)
                .Must(ContactgegevenType.CanParse)
                .WithMessage(t => $"De waarde '{t.Contactgegeven.Type}' is geen gekend contactgegeven type.")
        );
    }
}
