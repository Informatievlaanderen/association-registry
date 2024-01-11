namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using FluentValidation;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigMaatschappelijkeZetelRequestValidator : AbstractValidator<WijzigMaatschappelijkeZetelRequest>
{
    public WijzigMaatschappelijkeZetelRequestValidator()
    {
        RuleFor(request => request.Locatie)
           .NotNull()
           .WithMessage("'Locatie' is verplicht.");

        RuleFor(request => request.Locatie)
           .SetValidator(new TeWijzigenMaatschappelijkeZetelValidator());
    }
}
