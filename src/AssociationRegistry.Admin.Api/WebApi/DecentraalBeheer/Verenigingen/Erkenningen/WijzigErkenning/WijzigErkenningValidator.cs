namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning;

using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class WijzigErkenningValidator : AbstractValidator<WijzigErkenningRequest>
{
    public WijzigErkenningValidator()
    {
        RuleFor(request => request)
            .Must(HaveAtLeastOneValue)
            .OverridePropertyName("request")
            .WithMessage("Een request mag niet leeg zijn.");
    }

    private static bool HaveAtLeastOneValue(WijzigErkenningRequest request) =>
        request.Startdatum.HasValue
        || request.Einddatum.HasValue
        || request.Hernieuwingsdatum.HasValue
        || request.HernieuwingsUrl is not null;
}
