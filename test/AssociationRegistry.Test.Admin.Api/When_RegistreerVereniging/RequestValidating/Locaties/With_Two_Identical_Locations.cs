namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Identical_Locations : ValidatorTest
{
    [Fact]
    public void Has_validation_error__idenitiek_locaties_verboden()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var identiekLocatie = new RegistreerVerenigingRequest.Locatie
        {
            Locatietype = Locatietypes.Activiteiten,
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
                Copy(identiekLocatie),
                Copy(identiekLocatie),
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Locaties)
            .WithErrorMessage("Identieke locaties zijn niet toegelaten.");
    }

    private static T Copy<T>(T obj)
        => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj))!;
}
