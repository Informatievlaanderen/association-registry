namespace AssociationRegistry.Admin.Api.Verenigingen.Stop;

using FluentValidation;
using RequestModels;

public class StopVerenigingRequestValidator : AbstractValidator<StopVerenigingRequest>
{
    public StopVerenigingRequestValidator()
    {
        RuleFor(r => r.Einddatum)
           .NotNull()
           .WithMessage(string.Format(ValidationMessages.VeldIsVerplicht, nameof(StopVerenigingRequest.Einddatum)));
    }
}
