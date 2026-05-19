namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using FluentValidation;
using RegistreerErkenning;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class CorrigeerErkenningValidator : AbstractValidator<CorrigeerErkenningRequest>
{
    public CorrigeerErkenningValidator()
    {
        RuleFor(request => request)
            .Must(HaveAtLeastOneValue)
            .OverridePropertyName("request")
            .WithMessage("Een request mag niet leeg zijn.");
    }

    private static bool HaveAtLeastOneValue(CorrigeerErkenningRequest request) =>
        request.Startdatum.HasValue
        || request.Einddatum.HasValue
        || request.Hernieuwingsdatum.HasValue
        || request.HernieuwingsUrl is not null;
}
