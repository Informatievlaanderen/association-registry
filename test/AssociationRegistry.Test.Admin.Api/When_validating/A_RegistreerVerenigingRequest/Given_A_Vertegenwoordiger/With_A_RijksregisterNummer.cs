namespace AssociationRegistry.Test.Admin.Api.When_validating.A_RegistreerVerenigingRequest.Given_A_Vertegenwoordiger;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;

public class With_A_Rijksregisternummer
{
    [Fact]
    public void Then_it_has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Vertegenwoordigers = new []
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger()
                {
                    Rijksregisternummer = "78100440321",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
