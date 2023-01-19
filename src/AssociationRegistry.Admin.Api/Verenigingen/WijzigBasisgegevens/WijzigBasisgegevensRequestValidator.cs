// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

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
    }

    private static bool HaveAtLeastOneValue(WijzigBasisgegevensRequest request)
        => request.Naam is not null ||
           request.KorteNaam is not null;
}
