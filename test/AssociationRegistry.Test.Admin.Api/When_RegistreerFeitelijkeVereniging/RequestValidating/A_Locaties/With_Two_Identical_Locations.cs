namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using FluentValidation.TestHelper;
using Framework;
using Framework.Helpers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Identical_Locations : ValidatorTest
{
    [Fact]
    public void Has_validation_error__idenitiek_locaties_verboden()
    {
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var identiekLocatie = new ToeTeVoegenLocatie
        {
            Locatietype = Locatietypes.Activiteiten,
            Huisnummer = "23",
            Gemeente = "Zonnedorp",
            Postcode = "0123",
            Land = "Belgie",
        };
        var request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Locaties = new[]
            {
                identiekLocatie.Copy(),
                identiekLocatie.Copy(),
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Locaties)
            .WithErrorMessage("Identieke locaties zijn niet toegelaten.");
    }
}
