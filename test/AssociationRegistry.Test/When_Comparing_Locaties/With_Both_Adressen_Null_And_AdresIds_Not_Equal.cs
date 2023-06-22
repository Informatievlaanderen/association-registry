namespace AssociationRegistry.Test.When_Comparing_Locaties;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Both_Adressen_Null_And_AdresIds_Not_Equal
{
    [Fact]
    public void Then_it_returns_false()
    {
        var locatie1 = Locatie.Create(
            "naam",
            true,
            Locatietype.Activiteiten,
            AdresId.Create("AR", AdresId.DataVlaanderenAdresPrefix + "1"),
            null);

        var locatie2 = Locatie.Create(
            "naam",
            true,
            Locatietype.Activiteiten,
            AdresId.Create("AR", AdresId.DataVlaanderenAdresPrefix + "2"),
            null);

        locatie1.Equals(locatie2).Should().BeFalse();
    }
}
