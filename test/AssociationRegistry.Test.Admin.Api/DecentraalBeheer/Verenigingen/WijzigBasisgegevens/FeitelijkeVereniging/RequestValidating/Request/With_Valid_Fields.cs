namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestValidating.Request;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Primitives;
using FluentValidation.TestHelper;
using System.Globalization;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Fields
{
    [Theory]
    [InlineData(null, null, null, null, null, null, null, null, new []{"BE25"})]
    [InlineData("naam", null, null, null, null, null, null, null, null)]
    [InlineData(null, "korte naam", null, null, null, null, null, null, null)]
    [InlineData(null, null, "korte beschrijving", null, null, null, null, null, null)]
    [InlineData(null, null, null, false, null, null, null, null, null)]
    [InlineData(null, null, null, null, "1996-12-31", null, null, null, null)]
    [InlineData(null, null, null, null, null, new[] { "abcd" }, null, null, null)]
    [InlineData("naam", "korte naam", "korte beschrijving", true, "2000-03-22", new[] { "abcd" }, null, null, null)]
    [InlineData("naam", null, null, null, null, null, 12, null, null)]
    [InlineData("naam", null, null, null, null, null, null, 111, null)]
    [InlineData("naam", "korte naam", "korte beschrijving", true, "2000-03-22", new[] { "abcd" }, 12, 18, null)]
    public void Then_it_should_not_have_errors(
        string? naam,
        string? korteNaam,
        string? korteBeschrijving,
        bool? isUitgeschreven,
        string? startdatum,
        string[] hoofdactiviteiten,
        int? minimumLeeftijd,
        int? maximumLeeftijd,
        string[] werkingsgebieden)
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(
            new WijzigBasisgegevensRequest
            {
                Naam = naam,
                KorteNaam = korteNaam,
                KorteBeschrijving = korteBeschrijving,
                Startdatum = ToNullOrEmptyDateOnly(startdatum),
                Doelgroep = new DoelgroepRequest
                {
                    Minimumleeftijd = minimumLeeftijd,
                    Maximumleeftijd = maximumLeeftijd,
                },
                HoofdactiviteitenVerenigingsloket = hoofdactiviteiten,
                IsUitgeschrevenUitPubliekeDatastroom = isUitgeschreven,
                Werkingsgebieden = werkingsgebieden,
            });

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static NullOrEmpty<DateOnly> ToNullOrEmptyDateOnly(string? startdatum)
        => startdatum is null
            ? NullOrEmpty<DateOnly>.Null
            : NullOrEmpty<DateOnly>.Create(DateOnly.ParseExact(startdatum, format: "yyyy-MM-dd", CultureInfo.InvariantCulture));
}
