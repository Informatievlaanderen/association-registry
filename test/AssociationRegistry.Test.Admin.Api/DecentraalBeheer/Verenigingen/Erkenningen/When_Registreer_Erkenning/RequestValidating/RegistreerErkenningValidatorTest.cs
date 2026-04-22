namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Resources;
using Xunit;

public class RegistreerErkenningValidatorTest : ValidatorTest
{
    [Fact]
    public void With_Erkenning_Null_Then_Has_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();

        var request = new RegistreerErkenningRequest();

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning)
            .WithErrorMessage("'Erkenning' is verplicht.");
    }

    [Fact]
    public void With_IpdcProductnummer_Empty_Then_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();

        var request = new RegistreerErkenningRequest() { Erkenning = new() { IpdcProductNummer = "" } };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning.IpdcProductNummer)
            .WithErrorMessage($"'ipdcProductNummer' mag niet leeg zijn.");
    }

    [Fact]
    public void With_IpdcProductnummer_Null_Then_ValidationError()
    {
        var validator = new RegistreerErkenningValidator();

        var request = new RegistreerErkenningRequest() { Erkenning = new() { IpdcProductNummer = null } };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.Erkenning.IpdcProductNummer)
            .WithErrorMessage($"'ipdcProductNummer' is verplicht.");
    }
}
