namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.RequestValidating.A_KboNumber;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Too_Short : ValidatorTest
{
    [Fact]
    public void Has_validation_error__Kbo_moet_10_cijfers_bevatten()
    {
        var validator = new RegistreerVerenigingUitKboRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingUitKboRequest { KboNummer = "1234" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.KboNummer)
              .WithErrorMessage("'KboNummer' moet 10 cijfers bevatten.");
    }
}
