namespace AssociationRegistry.Test.When_Comparing_Locaties;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Naam_NotEqual
{
    [Fact]
    public void Then_it_returns_false()
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
            Locatienaam.Create("Anderenaam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix),
            Adres.Create(straatnaam: "straatnaam",
                         huisnummer: "huisnummer",
                         busnummer: "busnummer",
                         postcode: "postCode",
                         gemeente: "gemeente",
                         land: "land"));

        locatie1.Equals(locatie2).Should().BeFalse();
    }
}
