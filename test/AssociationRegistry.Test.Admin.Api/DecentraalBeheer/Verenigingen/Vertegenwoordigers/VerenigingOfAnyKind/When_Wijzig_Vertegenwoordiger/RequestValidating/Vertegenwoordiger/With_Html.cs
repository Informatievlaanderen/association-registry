namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.RequestValidating.Vertegenwoordiger;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_Html : ValidatorTest
{
    [Theory]
    [InlineData(nameof(TeWijzigenVertegenwoordiger.Rol))]
    [InlineData(nameof(TeWijzigenVertegenwoordiger.Roepnaam))]
    [InlineData(nameof(TeWijzigenVertegenwoordiger.Email))]
    [InlineData(nameof(TeWijzigenVertegenwoordiger.Telefoon))]
    [InlineData(nameof(TeWijzigenVertegenwoordiger.Mobiel))]
    [InlineData(nameof(TeWijzigenVertegenwoordiger.SocialMedia))]
    public void Has_validation_error__waarde_bevat_niet_toegestane_tekens(string property)
    {
        var validator = new WijzigVertegenwoordigerValidator();
        var vertegenwoordiger = new TeWijzigenVertegenwoordiger();
        typeof(TeWijzigenVertegenwoordiger).GetProperty(property)!.SetValue(vertegenwoordiger, "<b>Frodo</b>");

        var result = validator.TestValidate(
            new WijzigVertegenwoordigerRequest { Vertegenwoordiger = vertegenwoordiger }
        );

        result
            .ShouldHaveValidationErrorFor($"{nameof(WijzigVertegenwoordigerRequest.Vertegenwoordiger)}.{property}")
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_no_validation_errors_without_html()
    {
        var validator = new WijzigVertegenwoordigerValidator();

        var result = validator.TestValidate(
            new WijzigVertegenwoordigerRequest
            {
                Vertegenwoordiger = new TeWijzigenVertegenwoordiger
                {
                    Rol = "Ringdrager",
                    Roepnaam = "Mr. Underhill",
                    Email = "frodo@hobbitstee.be",
                    Telefoon = "011 42 01 20",
                    Mobiel = "0470 12 34 56",
                    SocialMedia = "https://social.hobbitstee.be/frodo",
                },
            }
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}
