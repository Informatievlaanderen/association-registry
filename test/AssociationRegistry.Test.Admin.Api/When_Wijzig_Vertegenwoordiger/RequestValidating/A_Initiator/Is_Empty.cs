namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.RequestValidating.A_Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.WijzigVertegenwoordiger;
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
        var validator = new WijzigVertegenwoordigerValidator();
        var result = validator.TestValidate(new WijzigVertegenwoordigerRequest { Initiator = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' mag niet leeg zijn.")
            .Only();
    }
}
