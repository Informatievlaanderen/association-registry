namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;

using FluentValidation;
using Infrastructure.Validation;
using Vereniging;

public class TeWijzigenMaatschappelijkeZetelValidator : AbstractValidator<TeWijzigenMaatschappelijkeZetel>
{
    public TeWijzigenMaatschappelijkeZetelValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .WithMessage("'Locatie' moet ingevuld zijn.");

        RuleFor(maatschappelijkeZetel => maatschappelijkeZetel.Naam).MustNotContainHtml();

        RuleFor(maatschappelijkeZetel => maatschappelijkeZetel.Naam)
           .MustNotBeMoreThanAllowedMaxLength(Locatie.MaxLengthLocatienaam,
                                              $"Locatienaam mag niet langer dan {Locatie.MaxLengthLocatienaam} karakters zijn.");
    }

    private bool HaveAtLeastOneValue(TeWijzigenMaatschappelijkeZetel locatie)
        => locatie.IsPrimair is not null ||
           locatie.Naam is not null;
}
