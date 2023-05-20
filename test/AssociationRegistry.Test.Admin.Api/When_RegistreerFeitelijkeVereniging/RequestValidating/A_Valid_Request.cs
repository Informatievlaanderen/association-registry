namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class A_Valid_Request : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerFeitelijkeVerenigingRequest { Naam = "abcd" });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
