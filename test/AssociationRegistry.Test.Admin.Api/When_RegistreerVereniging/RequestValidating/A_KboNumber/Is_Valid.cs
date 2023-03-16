namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_KboNumber;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
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
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { KboNummer = "1234567890"});

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.KboNummer);
    }
}
