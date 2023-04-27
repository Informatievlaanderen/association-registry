namespace AssociationRegistry.Test.Admin.Api.When_Removing_Vertegenwoordiger.RequestValidating.Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerwijderVertegenwoordiger;
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
        var validator = new VerwijderVertegenwoordigerRequestValidator();
        var result = validator.TestValidate(new VerwijderVertegenwoordigerRequest() { Initiator = "abcd" });

        result.ShouldNotHaveValidationErrorFor(request => request.Initiator);
    }
}
