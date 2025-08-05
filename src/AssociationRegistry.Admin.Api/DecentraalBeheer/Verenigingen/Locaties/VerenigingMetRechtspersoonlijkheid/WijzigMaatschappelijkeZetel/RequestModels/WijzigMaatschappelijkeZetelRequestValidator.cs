namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using FluentValidation;
using Infrastructure.WebApi.Validation;

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
