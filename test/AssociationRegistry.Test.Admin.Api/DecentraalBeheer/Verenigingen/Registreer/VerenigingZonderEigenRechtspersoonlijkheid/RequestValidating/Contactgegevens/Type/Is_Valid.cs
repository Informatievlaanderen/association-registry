﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.Contactgegevens.
    Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));

        var result = validator.TestValidate(
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Contactgegeventype = Contactgegeventype.Email,
                        },
                    },
            });

        result.ShouldNotHaveValidationErrorFor(
            $"{nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Contactgegeventype)}");
    }
}
