namespace AssociationRegistry.Test.Admin.Api.When_validating.A_WijzigBasisgegevensRequest.Given_A_Korte_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Then_it_has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { KorteNaam = "" });

        result.ShouldNotHaveValidationErrorFor(request => request.KorteNaam);
    }
}
