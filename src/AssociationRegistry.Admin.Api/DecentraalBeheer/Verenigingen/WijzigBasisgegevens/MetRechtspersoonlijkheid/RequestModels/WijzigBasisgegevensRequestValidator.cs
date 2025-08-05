// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;

using Common;
using FluentValidation;
using Infrastructure.WebApi.Validation;

public class WijzigBasisgegevensRequestValidator : AbstractValidator<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequestValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .OverridePropertyName("request")
           .WithMessage("Een request mag niet leeg zijn.");

        RuleFor(request => request.Roepnaam).MustNotContainHtml();

        RuleFor(request => request.KorteBeschrijving)
           .MustNotContainHtml();

        RuleForEach(request => request.HoofdactiviteitenVerenigingsloket).MustNotContainHtml();

        RuleFor(request => request.HoofdactiviteitenVerenigingsloket)
           .Must(NotHaveDuplicates!)
           .WithMessage("Elke waarde in de hoofdactiviteiten mag slechts 1 maal voorkomen.")
           .When(r => r.HoofdactiviteitenVerenigingsloket is not null);

        RuleFor(request => request.Werkingsgebieden)
           .SetValidator(new WerkingsgebiedenValidator());

        RuleFor(request => request.Doelgroep)
           .SetValidator(new DoelgroepRequestValidator()!)
           .When(r => r.Doelgroep is not null);
    }

    private static bool NotHaveDuplicates(string[]? values)
        => values is not null
            ? values.Length == values.DistinctBy(v => v.ToLower()).Count()
            : true;

    private static bool HaveAtLeastOneValue(WijzigBasisgegevensRequest request)
        => request.KorteBeschrijving is not null ||
           request.HoofdactiviteitenVerenigingsloket is not null ||
           request.Roepnaam is not null ||
           request.Doelgroep is not null ||
           request.Werkingsgebieden is not null;
}
