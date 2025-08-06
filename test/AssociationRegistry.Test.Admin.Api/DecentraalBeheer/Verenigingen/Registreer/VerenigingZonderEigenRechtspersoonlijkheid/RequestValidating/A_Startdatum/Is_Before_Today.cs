namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.
    A_Startdatum;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging.Exceptions;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Before_Today : ValidatorTest
{
    [Fact]
    public void Has_validation_error__startdatum_ligt_in_de_toekomst()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MinValue));

        var result = validator.TestValidate(new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            Startdatum = new DateOnly(year: 2023, month: 11, day: 21),
        });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Startdatum)
              .WithErrorMessage(new StartdatumMagNietInToekomstZijn().Message);
    }
}
