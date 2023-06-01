namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Vertegenwoordiger.With_A_Voornaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null
{
    [Fact]
    public void Has_validation_error__Voornaam_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Vertegenwoordigers = new []
            {
                new ToeTeVoegenVertegenwoordiger{
                    Achternaam = null!,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(RegistreerAfdelingRequest.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Voornaam)}")
            .WithErrorMessage("'Voornaam' is verplicht.");
    }
}
