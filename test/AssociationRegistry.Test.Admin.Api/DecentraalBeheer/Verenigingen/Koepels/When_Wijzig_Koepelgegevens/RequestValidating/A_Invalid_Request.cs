namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Koepels.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class A_Invalid_Request : ValidatorTest
{
    private readonly WijzigKoepelgegevensRequestValidator _validator = new();

    [Fact]
    public void Has_validation_error_when_request_is_empty()
    {
        var result = _validator.TestValidate(new WijzigKoepelgegevensRequest());

        result.ShouldHaveValidationErrorFor("request").WithErrorMessage("Een request mag niet leeg zijn.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("koepel")]
    [InlineData("Onbekend")]
    [InlineData("SUB")]
    public void Has_validation_error_when_koepelstatus_is_invalid(string koepelstatus)
    {
        var result = _validator.TestValidate(new WijzigKoepelgegevensRequest { Koepelstatus = koepelstatus });

        result
            .ShouldHaveValidationErrorFor(request => request.Koepelstatus)
            .WithErrorMessage(WijzigKoepelgegevensRequestValidator.KoepelstatusMoetGeldigZijn);
    }
}
