namespace AssociationRegistry.Test.Admin.Api.When_Removing_Vertegenwoordiger.RequestValidating.Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerwijderVertegenwoordiger;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__initiator_is_verplicht()
    {
        var validator = new VerwijderVertegenwoordigerRequestValidator();
        var result = validator.TestValidate(new VerwijderVertegenwoordigerRequest());

        result.ShouldHaveValidationErrorFor(request => request.Initiator)
            .WithErrorMessage("'Initiator' is verplicht.");
    }
}
