namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;


[UnitTest]
public class Without_A_Land : ValidatorTest
{
    [Fact]
    public void Has_validation_error__land_is_verplicht()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Locaties = new[]
            {
                new RegistreerVerenigingRequest.Locatie
                {
                    Locatietype = Locatietypes.Activiteiten,
                    Straatnaam = "Dezestraat",
                    Huisnummer = "23",
                    Postcode = "0123",
                    Gemeente = "Hottentot",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Land)}")
            .WithErrorMessage("'Land' is verplicht.");
    }
}
