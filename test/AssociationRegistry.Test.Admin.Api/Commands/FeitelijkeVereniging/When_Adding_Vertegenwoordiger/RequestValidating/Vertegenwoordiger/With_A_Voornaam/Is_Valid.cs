namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger.
    With_A_Voornaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid
{
    [Theory]
    [InlineData("Jef")]
    [InlineData("Marie")]
    [InlineData("@#(!i i")]
    public void Has_no_validation_errors(string voornaam)
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

        result.ShouldNotHaveValidationErrorFor(
            $"{nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger)}.{nameof(ToeTeVoegenVertegenwoordiger.Voornaam)}");
    }
}
