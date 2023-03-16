﻿namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestValidating.A_Korte_Beschrijving;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_korte_naam()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { KorteBeschrijving = null });

        result.ShouldNotHaveValidationErrorFor(nameof(WijzigBasisgegevensRequest.KorteBeschrijving));
    }
}
