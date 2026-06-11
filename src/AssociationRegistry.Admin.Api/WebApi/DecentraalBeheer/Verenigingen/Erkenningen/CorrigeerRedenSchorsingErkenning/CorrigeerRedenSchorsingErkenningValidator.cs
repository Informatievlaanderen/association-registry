namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerRedenSchorsingErkenning;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class CorrigeerRedenSchorsingErkenningValidator : AbstractValidator<CorrigeerRedenSchorsingErkenningRequest>
{
    public CorrigeerRedenSchorsingErkenningValidator()
    {
        this.RequireNotNullOrEmpty(erkenning => erkenning.RedenSchorsing);
    }
}
