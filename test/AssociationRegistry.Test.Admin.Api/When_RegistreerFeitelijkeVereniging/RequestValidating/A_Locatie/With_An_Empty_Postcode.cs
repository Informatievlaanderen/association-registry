namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Postcode : ValidatorTest
{
    [Fact]
    public void Has_validation_error__postcode_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
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

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerDecentraalBeheerdeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Postcode)}")
            .WithErrorMessage("'Postcode' mag niet leeg zijn.");
    }
}
