namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger.
    With_A_Achternaam;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null
{
    [Fact]
    public void Has_validation_error__Achternaam_mag_niet_leeg_zijn()
    {
        var validator = new VoegVertegenwoordigerToeValidator();

        var request = new VoegVertegenwoordigerToeRequest
        {
            Vertegenwoordiger = new ToeTeVoegenVertegenwoordiger
            {
                Achternaam = null!,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger)}.{nameof(ToeTeVoegenVertegenwoordiger.Achternaam)}")
              .WithErrorMessage("'achternaam' is verplicht.");
    }
}
