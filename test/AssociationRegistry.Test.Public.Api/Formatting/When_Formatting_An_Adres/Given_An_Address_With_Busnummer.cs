namespace AssociationRegistry.Test.Public.Api.Formatting.When_Formatting_An_Adres;

using Events;
using Formatters;
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
        var locatie = new Registratiedata.Locatie(
            1,
            "Activiteiten",
            true,
            string.Empty,
            new Registratiedata.Adres(
                straatnaam,
                huisnummer,
                busnummer,
                postcode,
                gemeente,
                land),
            null);

        locatie.Adres.ToAdresString().Should().Be($"{straatnaam} {huisnummer} bus {busnummer}, {postcode} {gemeente}, {land}");
    }
}
