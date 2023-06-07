// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid;

using System.Linq;
using FluentValidation;

public class WijzigBasisgegevensRequestValidator : AbstractValidator<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequestValidator()
    {
        RuleFor(request => request)
            .Must(HaveAtLeastOneValue)
            .OverridePropertyName("request")
            .WithMessage("Een request mag niet leeg zijn.");

        RuleFor(request => request.HoofdactiviteitenVerenigingsloket)
            .Must(NotHaveDuplicates!)
            .WithMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.")
            .When(r => r.HoofdactiviteitenVerenigingsloket is not null);
    }

    private static bool NotHaveDuplicates(string[] values)
        => values.Length == values.DistinctBy(v => v.ToLower()).Count();

    private static bool HaveAtLeastOneValue(WijzigBasisgegevensRequest request)
        => request.KorteBeschrijving is not null ||
           request.HoofdactiviteitenVerenigingsloket is not null;
}
