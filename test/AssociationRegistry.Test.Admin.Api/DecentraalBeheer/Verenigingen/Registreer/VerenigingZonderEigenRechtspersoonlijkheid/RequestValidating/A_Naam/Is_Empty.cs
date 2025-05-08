namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

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
