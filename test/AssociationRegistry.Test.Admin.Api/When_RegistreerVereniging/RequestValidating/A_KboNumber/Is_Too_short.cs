namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_KboNumber;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Too_Short : ValidatorTest
{
    [Fact]
    public void Has_validation_error__Kbo_moet_10_cijfers_bevatten()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { KboNummer = "1234" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.KboNummer)
            .WithErrorMessage("KboNummer moet 10 cijfers bevatten.");
    }
}
