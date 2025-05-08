namespace AssociationRegistry.Test.Locaties.When_Comparing_Locaties;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;

public class With_Adres_NotEqual_And_AdresId_NotEqual
{
    [Fact]
    public void Then_it_returns_true()
    {
        var locatie1 = Locatie.Create(
            Locatienaam.Create("naam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "1"),
            Adres.Create(straatnaam: "straatnaam",
                         huisnummer: "huisnummer",
                         busnummer: "busnummer",
                         postcode: "postCode",
                         gemeente: "gemeente",
                         land: "land"));

        var locatie2 = Locatie.Create(
            Locatienaam.Create("naam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "2"));

        locatie1.Equals(locatie2).Should().BeFalse();
    }
}
