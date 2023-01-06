﻿namespace AssociationRegistry.Test.Admin.Api.When_validating.A_CreateVerenigingRequest.Given_Initiator;

using FluentValidation.TestHelper;
using Framework;
using global::AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Then_it_has_validation_error__initiator_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Initiator = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' mag niet leeg zijn.")
            .Only();
    }
}
