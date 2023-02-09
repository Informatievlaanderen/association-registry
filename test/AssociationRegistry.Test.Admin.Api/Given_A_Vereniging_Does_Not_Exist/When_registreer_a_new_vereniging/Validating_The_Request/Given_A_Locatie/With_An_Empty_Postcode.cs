namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging.Validating_The_Request.Given_A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Postcode : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__postcode_mag_niet_leeg_zijn()
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
                    Locatietype = Locatietypes.Activiteiten,
                    Straatnaam = "Dezestraat",
                    Gemeente = "Zonnedorp",
                    Huisnummer = "23",
                    Land = "Belgie",
                    Postcode = string.Empty,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Postcode)}")
            .WithErrorMessage("'Postcode' mag niet leeg zijn.");
    }
}
