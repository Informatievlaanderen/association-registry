﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var result = validator.TestValidate(new RegistreerFeitelijkeVerenigingRequest { Naam = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Naam);
    }
}
