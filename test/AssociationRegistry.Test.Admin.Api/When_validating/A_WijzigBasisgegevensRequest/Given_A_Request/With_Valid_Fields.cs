﻿namespace AssociationRegistry.Test.Admin.Api.When_validating.A_WijzigBasisgegevensRequest.Given_A_Request;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Xunit;

public class With_Valid_Fields
{
    [Theory]
    [InlineData(null, null, "korte beschrijving")]
    [InlineData("naam", null, null)]
    [InlineData(null, "korte naam", null)]
    [InlineData("naam", "korte naam", "korte beschrijving")]
    public void Then_it_should_not_have_errors(string? naam, string? korteNaam, string? korteBeschrijving)
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(
            new WijzigBasisgegevensRequest
            {
                Naam = naam,
                KorteNaam = korteNaam,
                KorteBeschrijving = korteBeschrijving,
                Initiator = "ikki",
            });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
