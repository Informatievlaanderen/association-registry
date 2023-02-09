namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging.Validating_The_Request.Given_A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
