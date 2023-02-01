namespace AssociationRegistry.Test.Admin.Api.When_validating.A_RegistreerVerenigingRequest.Given_A_Vertegenwoordiger;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;

public class With_An_Insz
{
    [Theory]
    [InlineData("01131500149")]
    [InlineData("03.20.98-203.96")]
    public void Then_it_has_no_validation_errors(string insz)
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
                    Insz = insz,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
