namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using FluentValidation;
using Infrastructure.Validation;

public class TeWijzigenMaatschappelijkeZetelValidator : AbstractValidator<TeWijzigenMaatschappelijkeZetel>
{
    public TeWijzigenMaatschappelijkeZetelValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .WithMessage("'Locatie' moet ingevuld zijn.");

        RuleFor(maatschappelijkeZetel => maatschappelijkeZetel.Naam).MustNotContainHtml();
    }

    private bool HaveAtLeastOneValue(TeWijzigenMaatschappelijkeZetel locatie)
        => locatie.IsPrimair is not null ||
           locatie.Naam is not null;
}
