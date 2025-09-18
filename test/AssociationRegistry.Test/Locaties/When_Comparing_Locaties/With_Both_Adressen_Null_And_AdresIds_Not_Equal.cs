namespace AssociationRegistry.Test.Locaties.When_Comparing_Locaties;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using FluentAssertions;
using Xunit;

public class With_Both_Adressen_Null_And_AdresIds_Not_Equal
{
    [Fact]
    public void Then_it_returns_false()
    {
        var locatie1 = Locatie.Create(
            Locatienaam.Create("naam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(adresbron: "AR", AdresId.DataVlaanderenAdresPrefix + "1"));

        var locatie2 = Locatie.Create(
            Locatienaam.Create("naam"),
            isPrimair: true,
            Locatietype.Activiteiten,
            AdresId.Create(adresbron: "AR", AdresId.DataVlaanderenAdresPrefix + "2"));

        locatie1.Equals(locatie2).Should().BeFalse();
    }
}
