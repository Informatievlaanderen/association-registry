namespace AssociationRegistry.Test.Admin.Api.When_Validating.A_CreateVerenigingRequest.Given_Naam;

using FluentValidation.TestHelper;
using Framework;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Then_it_has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
    }
}
