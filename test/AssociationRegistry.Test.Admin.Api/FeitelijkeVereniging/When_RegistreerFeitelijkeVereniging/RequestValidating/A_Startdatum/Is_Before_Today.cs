﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Startdatum;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Is_Before_Today : ValidatorTest
{
    [Fact]
    public void Has_validation_error__startdatum_ligt_in_de_toekomst()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MinValue));

        var result = validator.TestValidate(new RegistreerFeitelijkeVerenigingRequest
        {
            Startdatum = new DateOnly(year: 2023, month: 11, day: 21),
        });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Startdatum)
              .WithErrorMessage(new StartdatumMagNietInToekomstZijn().Message);
    }
}
