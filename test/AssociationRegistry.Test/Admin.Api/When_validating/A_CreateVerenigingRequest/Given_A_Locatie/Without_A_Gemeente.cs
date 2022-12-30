namespace AssociationRegistry.Test.Admin.Api.When_Validating.A_CreateVerenigingRequest.Given_A_Locatie;

using FluentValidation.TestHelper;
using Framework;
using global::AssociationRegistry.Admin.Api.Constants;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Without_A_Gemeente : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__gemeente_is_verplicht()
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
                    LocatieType = LocatieTypes.Activiteiten,
                    Straatnaam = "Dezestraat",
                    Huisnummer = "23",
                    Postcode = "0123",
                    Land = "Belgie",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Gemeente)}")
            .WithErrorMessage("'Gemeente' is verplicht.");
    }
}
