namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Given_A_Locaties_Array_With_Two_Identical_Locations : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__idenitiek_locaties_verboden()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var identiekLocatie = new RegistreerVerenigingRequest.Locatie
        {
            LocatieType = LocatieTypes.Activiteiten,
            Huisnummer = "23",
            Gemeente = "Zonnedorp",
            Postcode = "0123",
            Land = "Belgie",
        };
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Locaties = new[]
            {
                identiekLocatie,
                identiekLocatie,
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Locaties)
            .WithErrorMessage("Identieke locaties zijn niet toegelaten.");
    }
}