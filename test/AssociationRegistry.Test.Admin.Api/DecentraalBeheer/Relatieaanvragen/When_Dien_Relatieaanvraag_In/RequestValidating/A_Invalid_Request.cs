namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.RequestValidating;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using FluentValidation.TestHelper;
using Resources;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

/// <summary>The vCodes and the relatieType are checked by their value objects: see To_DienRelatieaanvraagInCommand.</summary>
public class A_Invalid_Request : ValidatorTest
{
    private readonly DienRelatieaanvraagInRequestValidator _validator = new();

    [Theory]
    [InlineData(null, "V0001001", "Lidmaatschap")]
    [InlineData("V0001002", "", "Lidmaatschap")]
    [InlineData("V0001002", "V0001001", " ")]
    [InlineData(null, null, null)]
    public void Has_validation_error_when_a_required_field_is_missing(
        string? verenigingVCode,
        string? koepelVCode,
        string? relatieType
    )
    {
        var result = _validator.TestValidate(
            new DienRelatieaanvraagInRequest
            {
                VerenigingVCode = verenigingVCode!,
                KoepelVCode = koepelVCode!,
                RelatieType = relatieType!,
            }
        );

        result
            .ShouldHaveValidationErrorFor("request")
            .WithErrorMessage(ValidationMessages.RelatieaanvraagVerplichteVeldenOntbreken);
    }

    [Fact]
    public void Has_validation_error_when_the_beschrijving_contains_html()
    {
        var result = _validator.TestValidate(
            new DienRelatieaanvraagInRequest
            {
                KoepelVCode = "V0001001",
                VerenigingVCode = "V0001002",
                RelatieType = "Lidmaatschap",
                Beschrijving = "<p>Aanvraag</p>",
            }
        );

        result
            .ShouldHaveValidationErrorFor(request => request.Beschrijving)
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
