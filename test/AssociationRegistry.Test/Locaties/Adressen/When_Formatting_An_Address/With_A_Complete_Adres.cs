namespace AssociationRegistry.Test.Locaties.Adressen.When_Formatting_An_Address;

using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Complete_Adres
{
    [Fact]
    public void Then_It_Formats()
    {
        var straatnaam = "kerkstraat";
        var huisnummer = "1";
        var busnummer = "b2";
        var postcode = "007";
        var gemeente = "zonnedorp";
        var land = "fabeltjesland";

        var adres = new Registratiedata.Adres(
            straatnaam,
            huisnummer,
            busnummer,
            postcode,
            gemeente,
            land);

        var formatted = adres.ToAdresString();
        formatted.Should().Be("kerkstraat 1 bus b2, 007 zonnedorp, fabeltjesland");
    }
}
