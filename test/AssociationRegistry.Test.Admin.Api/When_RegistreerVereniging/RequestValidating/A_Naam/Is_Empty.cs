﻿namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__naam_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerVerenigingRequest { Naam = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Naam)
            .WithErrorMessage("'Naam' mag niet leeg zijn.")
            .Only();
    }
}
