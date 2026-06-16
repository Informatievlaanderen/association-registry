namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.Request;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.RequestModels;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Primitives;
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
              .WithErrorMessage(ExceptionMessages.MinstensEenTeWijzigenVeldMoetIngevuldZijn);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void With_RedenVanWijziging_Empty_Then_ValidationError(string redenVanWijziging)
    {
        var validator = new WijzigErkenningValidator();

        var request = new WijzigErkenningRequest() with
        {
            RedenVanWijziging = redenVanWijziging,
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(nameof(WijzigErkenningRequest.RedenVanWijziging))
              .WithErrorMessage(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }

    [Fact]
    public void With_Einddatum_Null_And_Startdatum_Null_Then_MinstensEenTeWijzigenVeldMoetIngevuldZijn()
    {
        var validator = new WijzigErkenningValidator();

        var request = new WijzigErkenningRequest
        {
            Startdatum = NullOrEmpty<DateOnly>.Null,
            Einddatum = NullOrEmpty<DateOnly>.Null,
            RedenVanWijziging = Fixture.Create<string>(),
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor("request")
              .WithErrorMessage(ExceptionMessages.MinstensEenTeWijzigenVeldMoetIngevuldZijn);
    }

    [Fact]
    public void With_Einddatum_Empty_Then_No_ValidationError()
    {
        var validator = new WijzigErkenningValidator();

        var request = new WijzigErkenningRequest() with
        {
            Einddatum = NullOrEmpty<DateOnly>.Empty,
            RedenVanWijziging = Fixture.Create<string>(),
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor("request");
    }

    [Fact]
    public void With_Startdatum_Empty_Then_No_ValidationError()
    {
        var validator = new WijzigErkenningValidator();

        var request = new WijzigErkenningRequest() with
        {
            Startdatum = NullOrEmpty<DateOnly>.Empty,
            RedenVanWijziging = Fixture.Create<string>(),
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor("request");
    }
}
