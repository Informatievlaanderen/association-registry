namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.RequestValidating.A_Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.WijzigVertegenwoordiger;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigVertegenwoordigerValidator();
        var result = validator.TestValidate(new WijzigVertegenwoordigerRequest() { Initiator = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Initiator);
    }
}
