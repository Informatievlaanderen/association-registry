namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;

using Infrastructure.Validation;
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
            request => request.Contactgegeven is not null,
            () => RuleFor(request => request.Contactgegeven)
                .Must(HaveAtLeastOneValue)
                .WithMessage("'Contactgegeven' moet ingevuld zijn.")
        );
        When(
            request => request.Contactgegeven is not null,
            () => this.RequireNotEmpty(request => request.Contactgegeven.Waarde)
        );
    }

    private static bool HaveAtLeastOneValue(TeWijzigenContactgegeven contactgegeven)
        => contactgegeven.Waarde is not null ||
           contactgegeven.Beschrijving is not null ||
           contactgegeven.IsPrimair is not null;
}
