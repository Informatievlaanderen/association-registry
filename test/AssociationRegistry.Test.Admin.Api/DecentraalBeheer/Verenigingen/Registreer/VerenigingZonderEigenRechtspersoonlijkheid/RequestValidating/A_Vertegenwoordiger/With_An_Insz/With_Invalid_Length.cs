namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Vertegenwoordiger.
    With_An_Insz;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_Invalid_Length
{
    [Theory]
    [InlineData("0123456789012")]
    [InlineData("0123456")]
    public void Has_validation_error__insz_moet_11_cijfers_bevatten(string insz)
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = insz,
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(request.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Insz)}")
              .WithErrorMessage("Insz moet 11 cijfers bevatten");
    }
}
