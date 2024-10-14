// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;

using Common;
using FluentValidation;
using Infrastructure.Validation;

public class WijzigBasisgegevensRequestValidator : AbstractValidator<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequestValidator()
    {
        RuleFor(request => request)
           .Must(HaveAtLeastOneValue)
           .OverridePropertyName("request")
           .WithMessage("Een request mag niet leeg zijn.");

        RuleFor(request => request.Naam).MustNotContainHtml();
        RuleFor(request => request.KorteNaam).MustNotContainHtml();

        RuleFor(request => request.KorteBeschrijving)
           .MustNotContainHtml();

        RuleFor(request => request.KorteBeschrijving).MustNotContainHtml();
        RuleForEach(request => request.HoofdactiviteitenVerenigingsloket).MustNotContainHtml();
        RuleForEach(request => request.Werkingsgebieden).MustNotContainHtml();

        RuleFor(request => request.Naam)
           .Must(naam => naam?.Trim() is null or not "")
           .WithMessage("'Naam' mag niet leeg zijn.");

        RuleFor(request => request.HoofdactiviteitenVerenigingsloket)
           .Must(NotHaveDuplicates!)
           .WithMessage("Elke waarde in de hoofdactiviteiten mag slechts 1 maal voorkomen.")
           .When(r => r.HoofdactiviteitenVerenigingsloket is not null);

        RuleFor(request => request.Werkingsgebieden)
           .Must(NotHaveDuplicates!)
           .WithMessage("Elke waarde in de werkingsgebieden mag slechts 1 maal voorkomen.")
           .When(r => r.Werkingsgebieden is not null);

        RuleFor(request => request.Doelgroep)
           .SetValidator(new DoelgroepRequestValidator()!)
           .When(r => r.Doelgroep is not null);
    }

    private static bool NotHaveDuplicates(string[] values)
        => values.Length == values.DistinctBy(v => v.ToLower()).Count();

    private static bool HaveAtLeastOneValue(WijzigBasisgegevensRequest request)
        => request.Naam is not null ||
           request.KorteNaam is not null ||
           request.KorteBeschrijving is not null ||
           request.IsUitgeschrevenUitPubliekeDatastroom is not null ||
           request.Startdatum is { IsNull: false } ||
           request.Doelgroep is not null ||
           request.HoofdactiviteitenVerenigingsloket is not null ||
           request.Werkingsgebieden is not null;
}
