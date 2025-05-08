namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.
    A_HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Has_Duplicates : ValidatorTest
{
    [Theory]
    [InlineData("ABCD", "ABCD")]
    [InlineData("Test", "tEST")]
    [InlineData("BLABLAbla", "BlAbLaBlA")]
    public void Has_a_validation_error_for_hoofdactiviteitenLijst(string hoofdactivitetiCode1, string hoofdactivitetiCode2)
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            HoofdactiviteitenVerenigingsloket = new[] { hoofdactivitetiCode1, hoofdactivitetiCode2 },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsloket)
              .WithErrorMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.");
    }
}
