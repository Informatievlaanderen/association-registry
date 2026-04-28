namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning;

using FluentValidation;
using Infrastructure.WebApi.Validation;
using RequestModels;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
public class SchorsErkenningValidator : AbstractValidator<SchorsErkenningRequest>
{
    public SchorsErkenningValidator()
    {
        this.RequireNotNullOrEmpty(erkenning => erkenning.RedenSchorsing);
    }
}
