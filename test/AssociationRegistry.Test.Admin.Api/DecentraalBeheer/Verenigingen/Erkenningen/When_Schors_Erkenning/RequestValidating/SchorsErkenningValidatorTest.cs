namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.
    RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class SchorsErkenningValidatorTest : ValidatorTest
{
    [Fact]
    public void With_ErkenningId_Null_Then_Has_ValidationError()
    {
        var validator = new SchorsErkenningValidator();

        var request = new SchorsErkenningRequest();

        var result = validator.TestValidate(request);

        result
           .ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning)
           .WithErrorMessage("'Erkenning' is verplicht.");
    }

    [Fact]
    public void With_IpdcProductnummer_Empty_Then_ValidationError()
    {
        var validator = new SchorsErkenningValidator();

        var request = new SchorsErkenningRequest() { Erkenning = new() { RedenSchorsing = "" } };

        var result = validator.TestValidate(request);

        result
           .ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning.RedenSchorsing)
           .WithErrorMessage($"'redenSchorsing' mag niet leeg zijn.");
    }

    [Fact]
    public void With_IpdcProductnummer_Null_Then_ValidationError()
    {
        var validator = new SchorsErkenningValidator();

        var request = new SchorsErkenningRequest() { Erkenning = new() { RedenSchorsing = null } };

        var result = validator.TestValidate(request);

        result
           .ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning.RedenSchorsing)
           .WithErrorMessage($"'redenSchorsing' is verplicht.");
    }
}
