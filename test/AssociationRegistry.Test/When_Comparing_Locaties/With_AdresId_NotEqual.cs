namespace AssociationRegistry.Test.When_Comparing_Locaties;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_AdresId_NotEqual
{
    [Fact]
    public void Then_it_returns_true()
    {
        var hetzelfdeAdres = Adres.Create("straatnaam",
            "huisnummer",
            "busnummer",
            "postCode",
            "gemeente",
            "land");

        var locatie1 = Locatie.Create(
            "naam",
            true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "1"),
            hetzelfdeAdres);

        var locatie2 = Locatie.Create(
            "naam",
            true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "2"),
            hetzelfdeAdres);

        locatie1.Equals(locatie2).Should().BeTrue();
    }
}
