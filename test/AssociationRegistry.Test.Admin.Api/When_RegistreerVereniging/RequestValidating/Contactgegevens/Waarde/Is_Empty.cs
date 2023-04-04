﻿namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.Contactgegevens.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenValue_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(
            new RegistreerVerenigingRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new RegistreerVerenigingRequest.Contactgegeven
                        {
                            Waarde = "",
                        },
                    },
            });

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Contactgegevens)}[0].{nameof(RegistreerVerenigingRequest.Contactgegeven.Waarde)}")
            .WithErrorMessage("'Waarde' mag niet leeg zijn.")
            .Only();
    }
}
