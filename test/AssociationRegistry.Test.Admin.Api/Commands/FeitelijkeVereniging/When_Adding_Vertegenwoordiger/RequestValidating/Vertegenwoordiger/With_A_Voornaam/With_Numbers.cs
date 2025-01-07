namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger
    .
    With_A_Voornaam;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Numbers
{
    [Theory]
    [InlineData("1234")]
    [InlineData("@#(!i i2")]
    public void Has_validation_error__Voornaam_mag_enkel_letters_bevatten(string voornaam)
    {
        var validator = new VoegVertegenwoordigerToeValidator();

        var request = new VoegVertegenwoordigerToeRequest
        {
            Vertegenwoordiger =
                new ToeTeVoegenVertegenwoordiger
                {
                    Voornaam = voornaam,
                },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger)}.{nameof(ToeTeVoegenVertegenwoordiger.Voornaam)}")
              .WithErrorMessage("'Voornaam' mag geen cijfers bevatten.");
    }
}
