namespace AssociationRegistry.Test.Locaties.When_Comparing_Locaties;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using FluentAssertions;
using Xunit;

public class With_All_Fields_Equal
{
    [Fact]
    public void Then_it_returns_true()
    {
        var locatie1 = Locatie.Create(
            Locatienaam.Create("naam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix),
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
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix),
            Adres.Create(straatnaam: "straatnaam",
                         huisnummer: "huisnummer",
                         busnummer: "busnummer",
                         postcode: "postCode",
                         gemeente: "gemeente",
                         land: "land"));

        locatie1.Equals(locatie2).Should().BeTrue();
    }
}
