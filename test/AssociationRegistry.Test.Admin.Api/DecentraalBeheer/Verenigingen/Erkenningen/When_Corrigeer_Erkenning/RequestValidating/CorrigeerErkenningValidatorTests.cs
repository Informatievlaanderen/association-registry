namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class CorrigeerErkenningValidatorTests : ValidatorTest
{
    [Fact]
    public void With_Empty_Request_Then_ValidationError()
    {
        var validator = new CorrigeerErkenningValidator();

        var request = new CorrigeerErkenningRequest();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("request").WithErrorMessage("Een request mag niet leeg zijn.");
    }
}
