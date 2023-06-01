namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Vertegenwoordiger.With_A_Achternaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Letter
{
    [Theory]
    [InlineData("@#(! ")]
    [InlineData("… --- …")]
    public void Has_validation_error__Achternaam_moet_minstens_een_letter_bevatten(string achternaam)
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Vertegenwoordigers = new []
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Achternaam = achternaam,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(RegistreerAfdelingRequest.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Achternaam)}")
            .WithErrorMessage("'Achternaam' moet minstens een letter bevatten.");
    }
}
