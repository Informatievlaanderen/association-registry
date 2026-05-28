namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.Request;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Resources;
using Xunit;

public class WijzigErkenningValidatorTests : ValidatorTest
{
    [Fact]
    public void With_Empty_Request_Then_ValidationError()
    {
        var validator = new WijzigErkenningValidator();

        var request = new WijzigErkenningRequest();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("request")
              .WithErrorMessage(ExceptionMessages.MinstensEenVeldMoetIngevuldZijn);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void With_Empty_RedenVanWijziging_Then_ValidationError(string redenVanWijziging)
    {
        var validator = new WijzigErkenningValidator();

        var request = new WijzigErkenningRequest() with
        {
            RedenVanWijziging = redenVanWijziging,
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("redenVanWijziging")
              .WithErrorMessage(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }
}
