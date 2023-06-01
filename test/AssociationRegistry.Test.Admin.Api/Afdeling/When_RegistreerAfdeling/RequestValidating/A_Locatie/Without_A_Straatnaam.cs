namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_A_Straatnaam : ValidatorTest
{
    [Fact]
    public void Has_validation_error__straatnaam_is_verplicht()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.Activiteiten,
                    Huisnummer = "23",
                    Gemeente = "Zonnedorp",
                    Postcode = "0123",
                    Land = "Belgie",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerAfdelingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Straatnaam)}")
            .WithErrorMessage("'Straatnaam' is verplicht.");
    }
}
