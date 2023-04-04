namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.Contactgegevens.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(
            new RegistreerVerenigingRequest()
            {
                Contactgegevens =
                    new[]
                    {
                        new RegistreerVerenigingRequest.Contactgegeven
                        {
                            Waarde = "test waarde",
                        },
                    },
            });

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Contactgegevens)}[0].{nameof(RegistreerVerenigingRequest.Contactgegeven.Waarde)}");
    }
}
