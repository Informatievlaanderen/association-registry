namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven;

using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigContactgegevenValidator : AbstractValidator<WijzigContactgegevenRequest>
{
    public WijzigContactgegevenValidator()
    {
        RuleFor(request => request.Contactgegeven).NotNull()
                                                  .WithMessage("'Contactgegeven' is verplicht.");

        When(
            predicate: request => request.Contactgegeven is not null,
            action: () => RuleFor(request => request.Contactgegeven)
                         .Must(HaveAtLeastOneValue)
                         .WithMessage("'Contactgegeven' moet ingevuld zijn.")
        );
    }

    private static bool HaveAtLeastOneValue(TeWijzigenContactgegeven contactgegeven)
        => contactgegeven.Beschrijving is not null ||
           contactgegeven.IsPrimair is not null;
}
