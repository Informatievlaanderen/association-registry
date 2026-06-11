namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerRedenSchorsingErkenning;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerRedenSchorsingErkenning.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class CorrigeerSchorsingValidatorTests : ValidatorTest
{
    [Fact]
    public void With_RedenSchorsing_Empty_Then_ValidationError()
    {
        var validator = new CorrigeerRedenSchorsingErkenningValidator();

        var request = new CorrigeerRedenSchorsingErkenningRequest() { RedenSchorsing = "" };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.RedenSchorsing)
            .WithErrorMessage($"'redenSchorsing' mag niet leeg zijn.");
    }

    [Fact]
    public void With_RedenSchorsing_Null_Then_ValidationError()
    {
        var validator = new CorrigeerRedenSchorsingErkenningValidator();

        var request = new CorrigeerRedenSchorsingErkenningRequest() { RedenSchorsing = null };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(toeRequest => toeRequest.RedenSchorsing)
            .WithErrorMessage($"'redenSchorsing' is verplicht.");
    }
}
