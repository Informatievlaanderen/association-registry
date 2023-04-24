namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestValidating.A_HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class IsNull : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_hoofdactiviteitenLijst()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var request = new WijzigBasisgegevensRequest
        {
            HoofdactiviteitenVerenigingsloket = null,
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsloket);
    }
}
