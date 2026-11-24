namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Weiger_Relatieaanvraag.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

/// <summary>The reden is optional: an empty request is valid.</summary>
public class A_Valid_Request : ValidatorTest
{
    private readonly WeigerRelatieaanvraagRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Wij zijn geen lid van deze koepel.")]
    public void Has_no_validation_errors(string? reden)
    {
        var result = _validator.TestValidate(new WeigerRelatieaanvraagRequest { Reden = reden });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
