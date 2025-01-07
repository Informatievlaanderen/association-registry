namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using FluentValidation;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigMaatschappelijkeZetelRequestValidator : AbstractValidator<WijzigMaatschappelijkeZetelRequest>
{
    public WijzigMaatschappelijkeZetelRequestValidator()
    {
        RuleFor(request => request.Locatie)
           .NotNull()
           .WithVeldIsVerplichtMessage(nameof(WijzigMaatschappelijkeZetelRequest.Locatie));

        RuleFor(request => request.Locatie)
           .SetValidator(new TeWijzigenMaatschappelijkeZetelValidator());
    }
}
