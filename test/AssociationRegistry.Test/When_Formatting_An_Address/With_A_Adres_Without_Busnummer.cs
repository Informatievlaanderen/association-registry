namespace AssociationRegistry.Test.When_Formatting_An_Address;

using Events;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Adres_Without_Busnummer
{
    [Fact]
    public void Then_It_Formats()
    {
        var straatnaam = "kerkstraat";
        var huisnummer = "1";
        var postcode = "007";
        var gemeente = "zonnedorp";
        var land = "fabeltjesland";

        var adres = new Registratiedata.Adres(
            straatnaam,
            huisnummer,
            string.Empty,
            postcode,
            gemeente,
            land);

        var formatted = adres.ToAdresString();
        formatted.Should().Be("kerkstraat 1, 007 zonnedorp, fabeltjesland");
    }
}
