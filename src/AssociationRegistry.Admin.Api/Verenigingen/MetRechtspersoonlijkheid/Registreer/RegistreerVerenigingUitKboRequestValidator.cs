// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.MetRechtspersoonlijkheid.Registreer;

using Infrastructure.Validation;
using FluentValidation;

public class RegistreerVerenigingUitKboRequestValidator : AbstractValidator<RegistreerVerenigingUitKboRequest>
{
    public RegistreerVerenigingUitKboRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.KboNummer);

        RuleFor(request => request.KboNummer)
            .Length(min: 10, int.MaxValue)
            .WithMessage("KboNummer moet 10 cijfers bevatten.")
            .When(request => !string.IsNullOrEmpty(request.KboNummer));
    }
}
