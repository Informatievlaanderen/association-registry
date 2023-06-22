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
            "naam",
            true,
            Locatietype.Activiteiten,
            null,
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
            null,
            Adres.Create("andere straatnaam",
                "huisnummer",
                "busnummer",
                "postCode",
                "gemeente",
                "land"));

        locatie1.Equals(locatie2).Should().BeFalse();
    }
}
