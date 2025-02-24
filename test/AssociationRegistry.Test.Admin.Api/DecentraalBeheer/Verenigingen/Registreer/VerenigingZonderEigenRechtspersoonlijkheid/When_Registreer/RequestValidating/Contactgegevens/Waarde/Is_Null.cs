namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.RequestValidating.Contactgegevens.
    Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequetsModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenValue_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));

        var result = validator.TestValidate(
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Waarde = null!,
                        },
                    },
            });

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Waarde)}")
              .WithErrorMessage("'Waarde' is verplicht.");
        // .Only();
    }
}
