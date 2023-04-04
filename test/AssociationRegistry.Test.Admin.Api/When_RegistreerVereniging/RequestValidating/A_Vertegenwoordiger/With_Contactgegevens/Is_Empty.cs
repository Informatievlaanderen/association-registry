namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_Contactgegevens;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegeven_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest()
        {
            Initiator = "OVO000001",
            Vertegenwoordigers =
                new[]
                {
                    new RegistreerVerenigingRequest.Vertegenwoordiger(),
                },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Vertegenwoordigers)}[0].{nameof(RegistreerVerenigingRequest.Vertegenwoordiger.Contactgegevens)}");
    }
}
