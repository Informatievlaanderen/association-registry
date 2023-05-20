namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
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
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerDecentraalBeheerdeVerenigingRequest { Naam = "abcd" });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
