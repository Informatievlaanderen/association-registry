namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Given_Initiator_Is_Empty_String : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__initiator_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Initiator = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' mag niet leeg zijn.")
            .Only();
    }
}
