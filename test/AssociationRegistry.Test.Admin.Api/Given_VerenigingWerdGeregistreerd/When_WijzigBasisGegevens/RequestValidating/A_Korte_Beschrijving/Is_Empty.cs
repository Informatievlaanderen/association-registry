namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.RequestValidating.A_Korte_Beschrijving;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { KorteBeschrijving = "" });

        result.ShouldNotHaveValidationErrorFor(request => request.KorteBeschrijving);
    }
}
