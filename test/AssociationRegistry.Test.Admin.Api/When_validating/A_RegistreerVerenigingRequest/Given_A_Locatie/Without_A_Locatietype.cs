namespace AssociationRegistry.Test.Admin.Api.When_validating.A_RegistreerVerenigingRequest.Given_A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Without_A_Locatietype : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__locatieType_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Locaties = new[]
            {
                new RegistreerVerenigingRequest.Locatie
                {
                    Straatnaam = "dezeStraat",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Locatietype)}")
            .WithErrorMessage("'Locatietype' is verplicht.");
    }
}
