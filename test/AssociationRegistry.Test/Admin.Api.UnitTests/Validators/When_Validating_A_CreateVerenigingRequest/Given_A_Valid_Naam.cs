namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Given_A_Valid_Naam : ValidatorTest
{
    [Fact]
    public void Then_it_has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
    }
}
