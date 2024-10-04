﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.
    A_WerkingsgebiedenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Has_Duplicates : ValidatorTest
{
    [Theory]
    [InlineData("BE", "be")]
    [InlineData("be25", "BE25")]
    [InlineData("Be2525", "bE2525")]
    public void Has_a_validation_error_for_werkingsgebiedenLijst(string werkingsgebiedenCode1, string werkingsgebiedenCode2)
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Werkingsgebieden = [werkingsgebiedenCode1, werkingsgebiedenCode2],
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden)
              .WithErrorMessage("Een waarde in de werkingsgebiedenLijst mag slechts 1 maal voorkomen.");
    }
}