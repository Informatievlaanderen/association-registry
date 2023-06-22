namespace AssociationRegistry.Test.When_Comparing_Locaties;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_IsPrimair_NotEqual
{
    [Fact]
    public void Then_it_returns_true()
    {
        var locatie1 = Locatie.Create(
            "naam",
            true,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "1"),
            Adres.Create("straatnaam",
                "huisnummer",
                "busnummer",
                "postCode",
                "gemeente",
                "land"));

        var locatie2 = Locatie.Create(
            "naam",
            false,
            Locatietype.Activiteiten,
            AdresId.Create(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "1"),
            Adres.Create("straatnaam",
                "huisnummer",
                "busnummer",
                "postCode",
                "gemeente",
                "land"));

        locatie1.Equals(locatie2).Should().BeTrue();
    }
}
