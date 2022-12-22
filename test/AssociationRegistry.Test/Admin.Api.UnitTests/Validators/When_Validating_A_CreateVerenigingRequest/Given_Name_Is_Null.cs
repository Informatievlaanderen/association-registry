namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Given_Name_Is_Null : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__naam_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Initiator = "OVO000001" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Naam)
            .WithErrorMessage("'Naam' is verplicht.");
    }
}
