﻿namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has__validation_errors__naam_kan_niet_leeg_zijn()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { Naam = "" });

        result.ShouldHaveValidationErrorFor(request => request.Naam)
            .WithErrorMessage("'Naam' mag niet leeg zijn.");
    }
}
