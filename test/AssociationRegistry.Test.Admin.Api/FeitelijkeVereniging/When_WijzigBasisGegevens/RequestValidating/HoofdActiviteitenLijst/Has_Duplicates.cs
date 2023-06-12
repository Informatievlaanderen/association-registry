﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Has_Duplicates : ValidatorTest
{
    [Theory]
    [InlineData("ABCD", "ABCD")]
    [InlineData("Test", "tEST")]
    [InlineData("BLABLAbla", "BlAbLaBlA")]
    public void Has_a_validation_error_for_hoofdactiviteitenLijst(string hoofdactivitetiCode1, string hoofdactivitetiCode2)
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var request = new WijzigBasisgegevensRequest
        {
            HoofdactiviteitenVerenigingsloket = new[] { hoofdactivitetiCode1, hoofdactivitetiCode2 },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsloket)
            .WithErrorMessage("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.");
    }
}
