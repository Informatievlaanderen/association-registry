namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class CorrigeerErkenningValidator : AbstractValidator<CorrigeerErkenningRequest>
{
    public CorrigeerErkenningValidator()
    {
    }
}
