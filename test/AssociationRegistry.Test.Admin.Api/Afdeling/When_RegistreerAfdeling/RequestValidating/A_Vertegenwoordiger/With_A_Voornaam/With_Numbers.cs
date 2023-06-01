namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Vertegenwoordiger.With_A_Voornaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Numbers
{
    [Theory]
    [InlineData("1234")]
    [InlineData("@#(!i i2")]
    public void Has_validation_error__Voornaam_mag_geen_cijfers_bevatten(string voornaam)
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Vertegenwoordigers = new []
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Voornaam = voornaam,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(RegistreerAfdelingRequest.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Voornaam)}")
            .WithErrorMessage("'Voornaam' mag geen cijfers bevatten.");
    }
}
