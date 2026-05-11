namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Schorsing_Erkenning.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class CorrigeerSchorsingValidatorTests : ValidatorTest
{
    [Fact]
    public void With_RedenSchorsing_Empty_Then_ValidationError()
    {
        var validator = new CorrigeerSchorsingErkenningValidator();

        var request = new CorrigeerSchorsingErkenningRequest() { RedenSchorsing = "" };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.RedenSchorsing)
            .WithErrorMessage($"'redenSchorsing' mag niet leeg zijn.");
    }

    [Fact]
    public void With_RedenSchorsing_Null_Then_ValidationError()
    {
        var validator = new CorrigeerSchorsingErkenningValidator();

        var request = new CorrigeerSchorsingErkenningRequest() { RedenSchorsing = null };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.RedenSchorsing)
            .WithErrorMessage($"'redenSchorsing' is verplicht.");
    }
}
