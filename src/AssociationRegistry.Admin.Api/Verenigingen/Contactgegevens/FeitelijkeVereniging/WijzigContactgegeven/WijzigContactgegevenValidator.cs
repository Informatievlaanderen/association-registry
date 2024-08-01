namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;

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
                                                  .WithMessage("'Contactgegeven' is verplicht.");

        RuleFor(request => request.Contactgegeven.Beschrijving)
           .MustNotBeMoreThanAllowedMaxLength(Contactgegeven.MaxLengthBeschrijving, $"Beschrijving mag niet langer dan {Contactgegeven.MaxLengthBeschrijving} karakters zijn.")
           .MustNotContainHtml();

        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => RuleFor(request => request.Contactgegeven)
                         .Must(HaveAtLeastOneValue)
                         .WithMessage("'Contactgegeven' moet ingevuld zijn.")
        );

        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => this.RequireNotEmpty(request => request.Contactgegeven.Waarde)
        );
    }

    private static bool HaveAtLeastOneValue(TeWijzigenContactgegeven contactgegeven)
        => contactgegeven.Waarde is not null ||
           contactgegeven.Beschrijving is not null ||
           contactgegeven.IsPrimair is not null;
}
