// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

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
        RuleFor(request => request.Naam)
            .Must(naam => naam?.Trim() is null or not "")
            .WithMessage("'Naam' mag niet leeg zijn.");
        RuleFor(request => request.HoofdactiviteitenVerenigingsloket)
            .Must(NotHaveDuplicates!)
            .WithMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.")
            .When(r => r.HoofdactiviteitenVerenigingsloket is not null);
    }

    private static bool NotHaveDuplicates(string[] values)
        => values.Length == values.DistinctBy(v => v.ToLower()).Count();

    private static bool HaveAtLeastOneValue(WijzigBasisgegevensRequest request)
        => request.Naam is not null ||
           request.KorteNaam is not null ||
           request.KorteBeschrijving is not null ||
           !request.Startdatum.IsNull ||
           request.HoofdactiviteitenVerenigingsloket is not null;
}
