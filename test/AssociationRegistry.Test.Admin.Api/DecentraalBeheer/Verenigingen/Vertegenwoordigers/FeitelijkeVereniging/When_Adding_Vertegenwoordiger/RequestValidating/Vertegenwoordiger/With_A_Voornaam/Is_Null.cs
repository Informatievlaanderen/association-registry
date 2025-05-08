namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger.
    With_A_Voornaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null
{
    [Fact]
    public void Has_validation_error__Voornaam_mag_niet_leeg_zijn()
    {
        var validator = new VoegVertegenwoordigerToeValidator();

        var request = new VoegVertegenwoordigerToeRequest
        {
            Vertegenwoordiger = new ToeTeVoegenVertegenwoordiger
            {
                Voornaam = null!,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger)}.{nameof(ToeTeVoegenVertegenwoordiger.Voornaam)}")
              .WithErrorMessage("'Voornaam' is verplicht.");
    }
}
