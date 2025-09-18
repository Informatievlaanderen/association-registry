namespace AssociationRegistry.Test.Locaties.When_Comparing_Locaties;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using FluentAssertions;
using Xunit;

public class With_IsPrimair_NotEqual
{
    [Fact]
    public void Then_They_Are_Not_Equal()
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
            isPrimair: false,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "1"),
            Adres.Create(straatnaam: "straatnaam",
                         huisnummer: "huisnummer",
                         busnummer: "busnummer",
                         postcode: "postCode",
                         gemeente: "gemeente",
                         land: "land"));

        locatie1.Equals(locatie2).Should().BeFalse();
    }
}
