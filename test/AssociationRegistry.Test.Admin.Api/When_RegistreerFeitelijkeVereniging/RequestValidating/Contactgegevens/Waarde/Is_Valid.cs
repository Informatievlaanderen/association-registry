namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.Contactgegevens.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
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
                            Waarde = "test waarde",
                        },
                    },
            });

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerDecentraalBeheerdeVerenigingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Waarde)}");
    }
}
