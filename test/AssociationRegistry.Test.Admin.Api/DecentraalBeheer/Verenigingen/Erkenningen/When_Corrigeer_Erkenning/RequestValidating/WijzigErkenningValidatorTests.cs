namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class WijzigErkenningValidatorTests : ValidatorTest
{
    [Fact]
    public void With_Empty_Request_Then_ValidationError()
    {
        var validator = new WijzigErkenningValidator();

        var request = new WijzigErkenningRequest();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("request").WithErrorMessage("Een request mag niet leeg zijn.");
    }
}
