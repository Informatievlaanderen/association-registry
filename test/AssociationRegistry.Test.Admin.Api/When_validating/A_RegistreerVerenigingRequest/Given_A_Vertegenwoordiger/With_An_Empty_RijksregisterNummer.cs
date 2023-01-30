namespace AssociationRegistry.Test.Admin.Api.When_validating.A_RegistreerVerenigingRequest.Given_A_Vertegenwoordiger;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Xunit;

public class With_An_Empty_Rijksregisternummer
{
    [Fact]
    public void Then_it_has_validation_error__rijksregisterNummer_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Vertegenwoordigers = new []
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger()
                {
                    Rijksregisternummer = string.Empty,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Vertegenwoordigers)}[0].{nameof(RegistreerVerenigingRequest.Vertegenwoordiger.Rijksregisternummer)}")
            .WithErrorMessage("'Rijksregisternummer' mag niet leeg zijn.");
    }
}
