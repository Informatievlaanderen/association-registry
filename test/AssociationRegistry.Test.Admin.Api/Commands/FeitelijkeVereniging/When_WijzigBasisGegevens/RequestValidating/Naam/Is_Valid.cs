namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.Naam;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { Naam = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
    }
}
