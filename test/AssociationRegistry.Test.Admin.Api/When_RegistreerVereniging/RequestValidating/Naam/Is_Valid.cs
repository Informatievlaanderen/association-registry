namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.Naam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
    }
}
