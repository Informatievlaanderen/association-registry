namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.Korte_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_korte_naam()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { KorteNaam = null });

        result.ShouldNotHaveValidationErrorFor(nameof(WijzigBasisgegevensRequest.KorteNaam));
    }
}
