namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.
    A_Vertegenwoordiger.
    With_A_Voornaam;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Numbers
{
    [Theory]
    [InlineData("1234")]
    [InlineData("@#(!i i2")]
    public void Has_validation_error__Voornaam_mag_geen_cijfers_bevatten(string voornaam)
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
              .WithErrorMessage("'Voornaam' mag geen cijfers bevatten.");
    }
}
