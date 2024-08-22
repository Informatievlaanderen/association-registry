namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.Request;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using FluentValidation.TestHelper;
using Primitives;
using System.Globalization;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Fields
{
    [Theory]
    [InlineData("naam", null, null, null, null, null, null, null)]
    [InlineData(null, "korte naam", null, null, null, null, null, null)]
    [InlineData(null, null, "korte beschrijving", null, null, null, null, null)]
    [InlineData(null, null, null, false, null, null, null, null)]
    [InlineData(null, null, null, null, "1996-12-31", null, null, null)]
    [InlineData(null, null, null, null, null, new[] { "abcd" }, null, null)]
    [InlineData("naam", "korte naam", "korte beschrijving", true, "2000-03-22", new[] { "abcd" }, null, null)]
    [InlineData("naam", null, null, null, null, null, 12, null)]
    [InlineData("naam", null, null, null, null, null, null, 111)]
    [InlineData("naam", "korte naam", "korte beschrijving", true, "2000-03-22", new[] { "abcd" }, 12, 18)]
    public void Then_it_should_not_have_errors(
        string? naam,
        string? korteNaam,
        string? korteBeschrijving,
        bool? isUitgeschreven,
        string? startdatum,
        string[] hoofdactiviteiten,
        int? minimumLeeftijd,
        int? maximumLeeftijd)
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
            });

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static NullOrEmpty<DateOnly> ToNullOrEmptyDateOnly(string? startdatum)
        => startdatum is null
            ? NullOrEmpty<DateOnly>.Null
            : NullOrEmpty<DateOnly>.Create(DateOnly.ParseExact(startdatum, format: "yyyy-MM-dd", CultureInfo.InvariantCulture));
}
