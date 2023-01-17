namespace AssociationRegistry.Test.Admin.Api.When_validating.A_RegistreerVerenigingRequest.Given_Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_Two_Different_Locations : ValidatorTest
{
    [Fact]
    public void Then_it_has_no_validation_error()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var identiekLocatie = new RegistreerVerenigingRequest.Locatie
        {
            Locatietype = Locatietypes.Activiteiten,
            Huisnummer = "23",
            Gemeente = "Zonnedorp",
            Postcode = "0123",
            Straatnaam = "Kerkstraat",
            Land = "Belgie",
        };
        var andereLocatie = new RegistreerVerenigingRequest.Locatie
        {
            Locatietype = Locatietypes.Activiteiten,
            Huisnummer = "23",
            Gemeente = "Anderdorp",
            Postcode = "0123",
            Straatnaam = "Kerkstraat",
            Land = "Belgie",
        };
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Locaties = new[]
            {
                identiekLocatie,
                andereLocatie,
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
