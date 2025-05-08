namespace AssociationRegistry.Test.Public.Api.Formatting.When_Formatting_An_Adres;

using Events;
using FluentAssertions;
using Formats;
using Xunit;

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

        var locatie = new Registratiedata.Locatie(
            LocatieId: 1,
            Locatietype: "Activiteiten",
            IsPrimair: true,
            string.Empty,
            new Registratiedata.Adres(
                straatnaam,
                huisnummer,
                string.Empty,
                postcode,
                gemeente,
                land),
            AdresId: null);

        locatie.Adres.ToAdresString().Should().Be($"{straatnaam} {huisnummer}, {postcode} {gemeente}, {land}");
    }
}
