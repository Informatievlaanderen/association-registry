namespace AssociationRegistry.Test.Admin.Api.UnitTests.Validators.When_Validating_A_CreateVerenigingRequest;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen;
using Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;

public class Given_A_Locatie_With_An_Invalid_LocatieType : ValidatorTest
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
                    LocatieType = new Fixture().Create<string>(),
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.LocatieType)}")
            .WithErrorMessage($"'LocatieType' moet een geldige waarde hebben. ({LocatieTypes.Correspondentie}, {LocatieTypes.Activiteiten}");
    }
}