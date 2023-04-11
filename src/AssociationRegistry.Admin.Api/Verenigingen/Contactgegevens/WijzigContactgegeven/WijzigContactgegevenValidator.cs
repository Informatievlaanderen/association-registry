namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;

using Infrastructure.Validation;
using FluentValidation;

public class WijzigContactgegevenValidator : AbstractValidator<WijzigContactgegevenRequest>
{
    public WijzigContactgegevenValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);

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

    private static bool HaveAtLeastOneValue(WijzigContactgegevenRequest.RequestContactgegeven contactgegeven)
        => contactgegeven.Waarde is not null ||
           contactgegeven.Omschrijving is not null ||
           contactgegeven.IsPrimair is not null;
}
