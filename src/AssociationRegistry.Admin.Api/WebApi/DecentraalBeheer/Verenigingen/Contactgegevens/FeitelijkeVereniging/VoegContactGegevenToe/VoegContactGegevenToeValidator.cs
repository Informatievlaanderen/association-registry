namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Vereniging;
using FluentValidation;
using RequestsModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VoegContactgegevenToeValidator : AbstractValidator<VoegContactgegevenToeRequest>
{
    public VoegContactgegevenToeValidator()
    {
        RuleFor(request => request.Contactgegeven)
           .NotNull()
           .WithVeldIsVerplichtMessage(nameof(VoegContactgegevenToeRequest.Contactgegeven));

        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => RuleFor(request => request.Contactgegeven.Beschrijving)
               .MustNotBeMoreThanAllowedMaxLength(Contactgegeven.MaxLengthBeschrijving,
                                                  $"Beschrijving mag niet langer dan {Contactgegeven.MaxLengthBeschrijving} karakters zijn.")
        );

        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => this.RequireNotNullOrEmpty(request => request.Contactgegeven.Waarde)
        );

        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => this.RequireNotNullOrEmpty(request => request.Contactgegeven.Contactgegeventype)
        );

        When(
            predicate: request => request.Contactgegeven is not null && !string.IsNullOrEmpty(request.Contactgegeven.Contactgegeventype),
            action: () => RuleFor(request => request.Contactgegeven.Contactgegeventype)
                         .Must(Contactgegeventype.CanParse)
                         .WithMessage(t => $"De waarde '{t.Contactgegeven.Contactgegeventype}' is geen gekend contactgegeven type.")
        );
    }
}
