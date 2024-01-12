namespace AssociationRegistry.Test.When_Comparing_Locaties;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Both_AdresIds_Null_And_Adres_Not_Equal
{
    [Fact]
    public void Then_it_returns_false()
    {
        var locatie1 = Locatie.Create(
            naam: "naam",
            isPrimair: true,
            Locatietype.Activiteiten,
            adresId: null,
            Adres.Create(straatnaam: "straatnaam",
                         huisnummer: "huisnummer",
                         busnummer: "busnummer",
                         postcode: "postCode",
                         gemeente: "gemeente",
                         land: "land"));

        var locatie2 = Locatie.Create(
            naam: "naam",
            isPrimair: true,
            Locatietype.Activiteiten,
            adresId: null,
            Adres.Create(straatnaam: "andere straatnaam",
                         huisnummer: "huisnummer",
                         busnummer: "busnummer",
                         postcode: "postCode",
                         gemeente: "gemeente",
                         land: "land"));

        locatie1.Equals(locatie2).Should().BeFalse();
    }
}
