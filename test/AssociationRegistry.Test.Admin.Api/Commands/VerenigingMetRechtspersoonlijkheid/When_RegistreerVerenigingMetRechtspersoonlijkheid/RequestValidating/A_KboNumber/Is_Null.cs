namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.RequestValidating.A_KboNumber;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__kboNummer_is_verplicht()
    {
        var validator = new RegistreerVerenigingUitKboRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingUitKboRequest());

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.KboNummer)
              .WithErrorMessage("'KboNummer' is verplicht.");
    }
}
