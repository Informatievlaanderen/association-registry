namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.Request;

using System.Globalization;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Primitives;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Fields
{
    [Theory]
    [InlineData("naam", null, null, null, null)]
    [InlineData(null, "korte naam", null, null, null)]
    [InlineData(null, null, "korte beschrijving", null, null)]
    [InlineData(null, null, null, "1996-12-31", null)]
    [InlineData(null, null, null, null, new[] {"abcd"})]
    [InlineData("naam", "korte naam", "korte beschrijving", "2000-03-22", new[] {"abcd"})]
    public void Then_it_should_not_have_errors(string? naam, string? korteNaam, string? korteBeschrijving, string? startdatum, string[] hoofdactiviteiten)
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(
            new WijzigBasisgegevensRequest
            {
                Naam = naam,
                KorteNaam = korteNaam,
                KorteBeschrijving = korteBeschrijving,
                Startdatum = ToNullOrEmptyDateOnly(startdatum),
                HoofdactiviteitenVerenigingsloket = hoofdactiviteiten,
            });

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static NullOrEmpty<DateOnly> ToNullOrEmptyDateOnly(string? startdatum)
        => startdatum is null ? NullOrEmpty<DateOnly>.Null : NullOrEmpty<DateOnly>.Create(DateOnly.ParseExact(startdatum, "yyyy-MM-dd", CultureInfo.InvariantCulture));
}
