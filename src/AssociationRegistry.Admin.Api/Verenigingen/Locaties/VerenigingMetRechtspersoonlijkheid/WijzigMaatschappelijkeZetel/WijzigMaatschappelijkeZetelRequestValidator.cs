namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;

using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigMaatschappelijkeZetelRequestValidator : AbstractValidator<WijzigMaatschappelijkeZetelRequest>
{
    public WijzigMaatschappelijkeZetelRequestValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .OverridePropertyName("request")
           .WithMessage("Een request mag niet leeg zijn.");
    }

    private static bool HaveAtLeastOneValue(WijzigMaatschappelijkeZetelRequest request)
        => request.Naam is not null ||
           request.IsPrimair is not null;
}
