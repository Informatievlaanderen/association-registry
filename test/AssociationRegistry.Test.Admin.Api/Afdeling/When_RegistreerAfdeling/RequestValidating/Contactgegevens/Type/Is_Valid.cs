﻿namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.Contactgegevens.Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using Vereniging;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var result = validator.TestValidate(
            new RegistreerAfdelingRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Type = ContactgegevenType.Email,
                        },
                    },
            });

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerAfdelingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Type)}");
    }
}
