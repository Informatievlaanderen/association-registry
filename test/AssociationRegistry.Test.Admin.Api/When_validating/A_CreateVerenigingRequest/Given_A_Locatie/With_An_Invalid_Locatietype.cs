namespace AssociationRegistry.Test.Admin.Api.When_validating.A_CreateVerenigingRequest.Given_A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_An_Invalid_Locatietype : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__locatieType_moet_juiste_waarde_hebben()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Locaties = new[]
            {
                new RegistreerVerenigingRequest.Locatie
                {
                    Locatietype = new Fixture().Create<string>(),
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.Locatietype)}")
            .WithErrorMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietypes.Correspondentie}, {Locatietypes.Activiteiten}");
    }
}
