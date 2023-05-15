namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Address_With_Busnummer
{
    [Fact]
    public void Then_Adres_contains_Busnummer()
    {
        const string straatnaam = "Den berg";
        const string huisnummer = "7";
        const string postcode = "9000";
        const string gemeente = "Gent";
        const string land = "België";
        const string busnummer = "1B";
        var locatie = new FeitelijkeVerenigingWerdGeregistreerd.Locatie(
            string.Empty,
            straatnaam,
            huisnummer,
            busnummer,
            postcode,
            gemeente,
            land,
            Hoofdlocatie: true,
            "Activiteiten");

        locatie.ToAdresString().Should().Be($"{straatnaam} {huisnummer} bus {busnummer}, {postcode} {gemeente}, {land}");
    }
}

[UnitTest]
public class Given_An_Address_Without_Busnummer
{
    [Fact]
    public void Then_Adres_does_not_contain_Busnummer()
    {
        const string straatnaam = "Den berg";
        const string huisnummer = "7";
        const string postcode = "9000";
        const string gemeente = "Gent";
        const string land = "België";

        var locatie = new FeitelijkeVerenigingWerdGeregistreerd.Locatie(
            string.Empty,
            straatnaam,
            huisnummer,
            string.Empty,
            postcode,
            gemeente,
            land,
            Hoofdlocatie: true,
            "Activiteiten");

        locatie.ToAdresString().Should().Be($"{straatnaam} {huisnummer}, {postcode} {gemeente}, {land}");
    }
}
