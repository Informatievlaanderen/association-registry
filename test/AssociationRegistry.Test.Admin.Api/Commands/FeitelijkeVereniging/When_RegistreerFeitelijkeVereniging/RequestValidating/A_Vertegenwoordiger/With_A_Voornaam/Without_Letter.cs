namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Vertegenwoordiger.
    With_A_Voornaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Letter
{
    [Theory]
    [InlineData("… --- …")]
    [InlineData("@#(!")]
    public void Has_validation_error__Voornaam_moet_minstens_een_letter_bevatten(string voornaam)
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Voornaam = voornaam,
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerFeitelijkeVerenigingRequest.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Voornaam)}")
              .WithErrorMessage("'Voornaam' moet minstens een letter bevatten.");
    }
}
