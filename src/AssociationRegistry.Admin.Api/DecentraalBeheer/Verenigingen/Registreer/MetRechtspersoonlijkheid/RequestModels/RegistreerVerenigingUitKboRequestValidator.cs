// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;

public class RegistreerVerenigingUitKboRequestValidator : AbstractValidator<RegistreerVerenigingUitKboRequest>
{
    public RegistreerVerenigingUitKboRequestValidator()
    {
        this.RequireValidKboNummer(request => request.KboNummer);
    }
}
