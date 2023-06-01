namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
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
        var validator = new RegistreerAfdelingRequestValidator();
        var result = validator.TestValidate(new RegistreerAfdelingRequest { Naam = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
    }
}
