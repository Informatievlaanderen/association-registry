﻿namespace AssociationRegistry.Test.Admin.Api.UnitTests.When_Validating.A_CreateVerenigingRequest.Given_A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_An_Empty_LocatieType : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__locatieType_mag_niet_leeg_zijn()
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
                    LocatieType = string.Empty,
                    Straatnaam = "dezeStraat",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(RegistreerVerenigingRequest.Locatie.LocatieType)}")
            .WithErrorMessage("'LocatieType' mag niet leeg zijn.");
    }
}
