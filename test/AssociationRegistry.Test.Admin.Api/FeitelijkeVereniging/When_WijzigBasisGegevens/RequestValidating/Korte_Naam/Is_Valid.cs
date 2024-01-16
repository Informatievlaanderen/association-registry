﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.Korte_Naam;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(new WijzigBasisgegevensRequest { KorteNaam = "abcd" });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.KorteNaam);
    }
}
