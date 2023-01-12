namespace AssociationRegistry.Test.Admin.Api.When_validating.A_CreateVerenigingRequest.Given_A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_A_Valid_Locatietype : ValidatorTest
{
    [Theory]
    [InlineData(Locatietypes.Correspondentie)]
    [InlineData(Locatietypes.Activiteiten)]
    public void Then_it_has_no_validation_errors(string locationType)
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
                    Locatietype = locationType,
                    Straatnaam = "dezeStraat",
                    Huisnummer = "23",
                    Gemeente = "Zonnedorp",
                    Postcode = "0123",
                    Land = "Belgie",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
