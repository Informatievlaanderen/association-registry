namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class A_Valid_Request : ValidatorTest
{
    private readonly DienRelatieaanvraagInRequestValidator _validator = new();

    [Theory]
    [InlineData("Lidmaatschap", "Aanvraag lidmaatschap bij de koepel")]
    [InlineData("Subvereniging", "Wij zijn de afdeling Bree")]
    [InlineData("lidmaatschap", null)]
    [InlineData("SUBVERENIGING", "")]
    public void Has_no_validation_errors(string relatieType, string? beschrijving)
    {
        var result = _validator.TestValidate(
            new DienRelatieaanvraagInRequest
            {
                KoepelVCode = "V0001001",
                VerenigingVCode = "V0001002",
                RelatieType = relatieType,
                Beschrijving = beschrijving,
            }
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}
