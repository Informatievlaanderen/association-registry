namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class SchorsErkenningValidatorTest : ValidatorTest
{
    [Fact]
    public void With_RedenSchorsing_Empty_Then_ValidationError()
    {
        var validator = new SchorsErkenningValidator();

        var request = new SchorsErkenningRequest() { RedenSchorsing = "" };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.RedenSchorsing)
            .WithErrorMessage($"'redenSchorsing' mag niet leeg zijn.");
    }

    [Fact]
    public void With_RedenSchorsing_Null_Then_ValidationError()
    {
        var validator = new SchorsErkenningValidator();

        var request = new SchorsErkenningRequest() { RedenSchorsing = null };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.RedenSchorsing)
            .WithErrorMessage($"'redenSchorsing' is verplicht.");
    }
}
