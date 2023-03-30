namespace AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;

using Infrastructure.Validation;
using FluentValidation;

public class VoegContactgegevenToeValidator : AbstractValidator<VoegContactgegevenToeRequest>
{
    public VoegContactgegevenToeValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);

        RuleFor(request => request.Contactgegeven).NotNull();
        When(request => request.Contactgegeven is not null, () => this.RequireNotNullOrEmpty(request => request.Contactgegeven.Waarde));
    }
}
