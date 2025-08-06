namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger.
    With_An_Insz;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using FluentValidation.TestHelper;
using Xunit;

public class With_Invalid_Characters
{
    [Theory]
    [InlineData("ABC.DEF")]
    [InlineData("25/03/71 123 57")]
    public void Has_validation_error__insz_heeft_incorect_formaat(string insz)
    {
        var validator = new VoegVertegenwoordigerToeValidator();

        var request = new VoegVertegenwoordigerToeRequest
        {
            Vertegenwoordiger =
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = insz,
                },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger)}.{nameof(ToeTeVoegenVertegenwoordiger.Insz)}")
              .WithErrorMessage("Insz heeft incorrect formaat (00.00.00-000.00 of 00000000000)");
    }
}
