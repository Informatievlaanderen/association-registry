namespace AssociationRegistry.Test.Admin.Api.UnitTests.When_Validating.A_CreateVerenigingRequest.Given_Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen;
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
            LocatieType = LocatieTypes.Activiteiten,
            Huisnummer = "23",
            Gemeente = "Zonnedorp",
            Postcode = "0123",
            Straatnaam = "Kerkstraat",
            Land = "Belgie",
        };
        var andereLocatie = new RegistreerVerenigingRequest.Locatie
        {
            LocatieType = LocatieTypes.Activiteiten,
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
