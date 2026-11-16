namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using AssociationRegistry.Resources;
using FluentValidation.TestHelper;
using Xunit;

public class With_Html
{
    [Theory]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Insz))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Voornaam))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Achternaam))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Rol))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Roepnaam))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Email))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Telefoon))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.Mobiel))]
    [InlineData(nameof(ToeTeVoegenVertegenwoordiger.SocialMedia))]
    public void Has_validation_error__waarde_bevat_niet_toegestane_tekens(string property)
    {
        var validator = new VoegVertegenwoordigerToeValidator();
        var vertegenwoordiger = new ToeTeVoegenVertegenwoordiger();
        typeof(ToeTeVoegenVertegenwoordiger).GetProperty(property)!.SetValue(vertegenwoordiger, "<b>Frodo</b>");

        var result = validator.TestValidate(
            new VoegVertegenwoordigerToeRequest { Vertegenwoordiger = vertegenwoordiger }
        );

        result
            .ShouldHaveValidationErrorFor($"{nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger)}.{property}")
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
