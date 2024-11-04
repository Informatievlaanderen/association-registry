namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven;

using FluentValidation;
using Infrastructure.Validation;
using RequestModels;
using Vereniging;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigContactgegevenValidator : AbstractValidator<WijzigContactgegevenRequest>
{
    public WijzigContactgegevenValidator()
    {
        RuleFor(request => request.Contactgegeven).NotNull()
                                                  .WithVeldIsVerplichtMessage(nameof(WijzigContactgegevenRequest.Contactgegeven));


        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => RuleFor(request => request.Contactgegeven)
                         .Must(HaveAtLeastOneValue)
                         .WithMessage("'Contactgegeven' moet ingevuld zijn.")
        );

        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => RuleFor(request => request.Contactgegeven.Beschrijving)
               .MustNotBeMoreThanAllowedMaxLength(Contactgegeven.MaxLengthBeschrijving,
                                                  $"Beschrijving mag niet langer dan {Contactgegeven.MaxLengthBeschrijving} karakters zijn.")
        );
    }

    private static bool HaveAtLeastOneValue(TeWijzigenContactgegeven contactgegeven)
        => contactgegeven.Beschrijving is not null ||
           contactgegeven.IsPrimair is not null;
}
