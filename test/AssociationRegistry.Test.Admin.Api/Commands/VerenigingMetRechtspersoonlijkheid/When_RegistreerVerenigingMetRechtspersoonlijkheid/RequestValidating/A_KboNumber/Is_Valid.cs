namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.RequestValidating.A_KboNumber;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
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
        var validator = new RegistreerVerenigingUitKboRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingUitKboRequest { KboNummer = "1234567890" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.KboNummer);
    }
}
