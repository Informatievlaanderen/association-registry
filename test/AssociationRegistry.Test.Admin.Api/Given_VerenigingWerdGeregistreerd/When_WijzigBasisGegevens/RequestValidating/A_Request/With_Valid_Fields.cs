﻿namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.RequestValidating.A_Request;

using System.Globalization;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Primitives;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Fields
{
    [Theory]
    [InlineData("naam", null, null, null)]
    [InlineData(null, "korte naam", null, null)]
    [InlineData(null, null, "korte beschrijving", null)]
    [InlineData(null, null, null, "1996-12-31")]
    [InlineData("naam", "korte naam", "korte beschrijving", "2000-03-22")]
    public void Then_it_should_not_have_errors(string? naam, string? korteNaam, string? korteBeschrijving, string? startdatum)
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(
            new WijzigBasisgegevensRequest
            {
                Naam = naam,
                KorteNaam = korteNaam,
                KorteBeschrijving = korteBeschrijving,
                Startdatum = ToNullOrEmptyDateOnly(startdatum),
                Initiator = "ikki",
            });

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static NullOrEmpty<DateOnly> ToNullOrEmptyDateOnly(string? startdatum)
        => startdatum is null ? NullOrEmpty<DateOnly>.Null : NullOrEmpty<DateOnly>.Create(DateOnly.ParseExact(startdatum, "yyyy-MM-dd", CultureInfo.InvariantCulture));
}
