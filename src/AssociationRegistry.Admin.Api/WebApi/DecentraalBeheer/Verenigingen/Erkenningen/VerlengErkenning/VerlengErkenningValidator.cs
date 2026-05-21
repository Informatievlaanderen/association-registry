namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.VerlengErkenning;

using FluentValidation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class VerlengErkenningValidator : AbstractValidator<VerlengErkenningRequest>
{
    public VerlengErkenningValidator()
    {
    }
}
