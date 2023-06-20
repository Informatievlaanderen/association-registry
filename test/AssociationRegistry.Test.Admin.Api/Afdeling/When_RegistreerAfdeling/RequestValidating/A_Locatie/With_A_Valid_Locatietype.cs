namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Valid_Locatietype : ValidatorTest
{
    [Theory]
    [InlineData(Locatietypes.Correspondentie)]
    [InlineData(Locatietypes.Activiteiten)]
    public void Has_no_validation_errors(string locationType)
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = locationType,
                    Adres = new ToeTeVoegenAdres
                    {
                        Straatnaam = "dezeStraat",
                        Huisnummer = "23",
                        Gemeente = "Zonnedorp",
                        Postcode = "0123",
                        Land = "Belgie",
                    },
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerAfdelingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Adres.Gemeente)}");
    }
}
