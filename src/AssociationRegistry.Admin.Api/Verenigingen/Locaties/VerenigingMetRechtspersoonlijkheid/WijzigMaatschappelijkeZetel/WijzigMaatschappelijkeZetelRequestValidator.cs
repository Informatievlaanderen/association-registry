namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;

using FluentValidation;
using RequestModels;

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

public class TeWijzigenMaatschappelijkeZetelValidator : AbstractValidator<TeWijzigenMaatschappelijkeZetel>
{
    public TeWijzigenMaatschappelijkeZetelValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .WithMessage("'Locatie' moet ingevuld zijn.");
    }

    private bool HaveAtLeastOneValue(TeWijzigenMaatschappelijkeZetel locatie)
        => locatie.IsPrimair is not null ||
           locatie.Naam is not null;
}
