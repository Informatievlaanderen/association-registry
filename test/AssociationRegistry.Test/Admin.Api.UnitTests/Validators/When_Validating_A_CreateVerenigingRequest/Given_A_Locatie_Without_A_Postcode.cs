namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Given_A_Locatie_Without_A_Postcode : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__postcode_is_verplicht()
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
                    Gemeente = "Zonnedorp",
                    Huisnummer = "23",
                    Land = "Belgie",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Postcode)}")
            .WithErrorMessage("'Postcode' is verplicht.");
    }
}