namespace AssociationRegistry.Test.When_Comparing_Locaties;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]

public class With_Adres_NotEqual_And_AdresId_Equal
{
    [Fact]
    public void Then_it_returns_true()
    {
        var dezelfdeAdresIdWaarde = AdresId.DataVlaanderenAdresPrefix;

        var locatie1 = Locatie.Create(
            "naam",
            true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, dezelfdeAdresIdWaarde),
            Adres.Create("straatnaam",
                "huisnummer",
                "busnummer",
                "postCode",
                "gemeente",
                "land"));

        var locatie2 = Locatie.Create(
            "naam",
            true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, dezelfdeAdresIdWaarde));

        locatie1.Equals(locatie2).Should().BeTrue();
    }
}
