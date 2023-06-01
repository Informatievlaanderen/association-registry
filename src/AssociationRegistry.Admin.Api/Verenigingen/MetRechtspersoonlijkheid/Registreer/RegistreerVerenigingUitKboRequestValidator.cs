// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.MetRechtspersoonlijkheid.Registreer;

using Infrastructure.Validation;
using FluentValidation;

public class RegistreerVerenigingUitKboRequestValidator : AbstractValidator<RegistreerVerenigingUitKboRequest>
{
    public RegistreerVerenigingUitKboRequestValidator()
    {
        this.RequireValidKboNummer(request => request.KboNummer);
    }
}
