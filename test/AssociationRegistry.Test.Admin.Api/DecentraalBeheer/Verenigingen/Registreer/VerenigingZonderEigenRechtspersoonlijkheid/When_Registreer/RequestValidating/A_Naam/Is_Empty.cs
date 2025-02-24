namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequetsModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__naam_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var result = validator.TestValidate(new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest { Naam = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Naam)
              .WithErrorMessage("'Naam' mag niet leeg zijn.");
        // .Only();
    }
}
