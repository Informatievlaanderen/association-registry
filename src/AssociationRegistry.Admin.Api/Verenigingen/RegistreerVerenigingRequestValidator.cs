namespace AssociationRegistry.Admin.Api.Verenigingen;

using FluentValidation;

public class RegistreerVerenigingRequestValidator : AbstractValidator<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequestValidator()
    {
        RuleFor(request => request.Naam)
            .NotNull()
            .WithMessage($"'{nameof(RegistreerVerenigingRequest.Naam)}' is verplicht.");
        RuleFor(request => request.Naam)
            .NotEmpty()
            .WithMessage($"'{nameof(RegistreerVerenigingRequest.Naam)}' mag niet leeg zijn.")
            .When(request => request.Naam is { });
    }
}
