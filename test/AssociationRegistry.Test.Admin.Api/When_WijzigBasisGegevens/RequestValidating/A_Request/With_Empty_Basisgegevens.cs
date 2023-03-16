namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestValidating.A_Request;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Empty_Basisgegevens
{
    [Fact]
    public void Then_it_should_have_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { Naam = null, KorteNaam = null});

        result.ShouldHaveValidationErrorFor("request")
            .WithErrorMessage("Een request mag niet leeg zijn.");
    }
}
