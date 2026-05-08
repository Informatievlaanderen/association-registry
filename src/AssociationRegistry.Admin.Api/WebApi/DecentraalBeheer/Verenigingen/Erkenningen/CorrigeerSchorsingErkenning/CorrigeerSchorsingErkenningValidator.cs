namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class CorrigeerSchorsingErkenningValidator : AbstractValidator<CorrigeerSchorsingErkenningRequest>
{
    public CorrigeerSchorsingErkenningValidator()
    {
        this.RequireNotNullOrEmpty(erkenning => erkenning.RedenSchorsing);
    }
}
