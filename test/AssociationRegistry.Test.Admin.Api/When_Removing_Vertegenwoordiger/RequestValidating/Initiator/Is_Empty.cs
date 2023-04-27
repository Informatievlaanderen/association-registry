namespace AssociationRegistry.Test.Admin.Api.When_Removing_Vertegenwoordiger.RequestValidating.Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerwijderVertegenwoordiger;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__initiator_mag_niet_leeg_zijn()
    {
        var validator = new VerwijderVertegenwoordigerRequestValidator();
        var result = validator.TestValidate(new VerwijderVertegenwoordigerRequest() { Initiator = "" });

        result.ShouldHaveValidationErrorFor(request => request.Initiator)
            .WithErrorMessage("'Initiator' mag niet leeg zijn.")
            .Only();
    }
}
