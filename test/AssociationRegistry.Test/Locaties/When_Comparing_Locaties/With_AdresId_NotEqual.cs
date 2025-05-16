namespace AssociationRegistry.Test.Locaties.When_Comparing_Locaties;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_AdresId_NotEqual
{
    [Fact]
    public void Then_it_returns_true()
    {
        var hetzelfdeAdres = Adres.Create(straatnaam: "straatnaam",
                                          huisnummer: "huisnummer",
                                          busnummer: "busnummer",
                                          postcode: "postCode",
                                          gemeente: "gemeente",
                                          land: "land");

        var locatie1 = Locatie.Create(
            Locatienaam.Create("naam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "1"),
            hetzelfdeAdres);

        var locatie2 = Locatie.Create(
            Locatienaam.Create("naam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "2"),
            hetzelfdeAdres);

        locatie1.Equals(locatie2).Should().BeTrue();
    }
}
