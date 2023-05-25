namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.Contactgegevens.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenValue_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var result = validator.TestValidate(
            new RegistreerDecentraalBeheerdeVerenigingRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Waarde = "",
                        },
                    },
            });

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerDecentraalBeheerdeVerenigingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Waarde)}")
            .WithErrorMessage("'Waarde' mag niet leeg zijn.")
            .Only();
    }
}
