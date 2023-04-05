namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_Contactgegevens.Type;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using ContactGegevens;
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
            new RegistreerVerenigingRequest
            {
                Vertegenwoordigers =
                    new[]
                    {
                        new RegistreerVerenigingRequest.Vertegenwoordiger
                        {
                            Contactgegevens =
                                new[]
                                {
                                    new RegistreerVerenigingRequest.Contactgegeven
                                    {
                                        Type = ContactgegevenType.Email,
                                    },
                                },
                        },
                    },
            });

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Vertegenwoordigers)}[0].{nameof(RegistreerVerenigingRequest.Vertegenwoordiger.Contactgegevens)}[0].{nameof(RegistreerVerenigingRequest.Contactgegeven.Type)}");
    }
}
