namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.Contactgegevens.Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;
using Fakes;
using FluentValidation.TestHelper;
using Framework;
using Vereniging;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerAfdelingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var result = validator.TestValidate(
            new RegistreerAfdelingRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Type = Contactgegeventype.Email,
                        },
                    },
            });

        result.ShouldNotHaveValidationErrorFor(
            $"{nameof(RegistreerAfdelingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Type)}");
    }
}
