namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Bevestig_Relatieaanvraag.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

/// <summary>Every field is optional: an empty request is valid.</summary>
public class A_Valid_Request : ValidatorTest
{
    private readonly BevestigRelatieaanvraagRequestValidator _validator = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("LID-1", "Lid van de koepel")]
    public void Has_no_validation_errors(string? identificatie, string? beschrijving)
    {
        var result = _validator.TestValidate(
            new BevestigRelatieaanvraagRequest { Identificatie = identificatie, Beschrijving = beschrijving }
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}
