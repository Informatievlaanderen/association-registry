namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Trek_Relatieaanvraag_In.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

/// <summary>The reden is optional: an empty request is valid.</summary>
public class A_Valid_Request : ValidatorTest
{
    private readonly TrekRelatieaanvraagInRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("De aanvraag werd per vergissing ingediend.")]
    public void Has_no_validation_errors(string? reden)
    {
        var result = _validator.TestValidate(new TrekRelatieaanvraagInRequest { Reden = reden });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
