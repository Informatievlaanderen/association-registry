namespace AssociationRegistry.Test.When_Comparing_Locaties;

using Admin.Api.Constants;
using FluentAssertions;
using Vereniging;
using Xunit;

public class With_AdressId_Equal_But_Not_AdresComponenten
{
    [Fact]
    public void Then_it_returns_true()
    {
        var locatie1 = Locatie.Create(
            "naam",
            true,
            Locatietypes.Activiteiten,
            AdresId.Create(Adresbron.AR, "waarde"),
            Adres.Create("straatnaam",
                "huisnummer",
                "busnummer",
                "postCode",
                "gemeente",
                "land"));

        var locatie2 = Locatie.Create(
            "naam",
            true,
            Locatietypes.Activiteiten,
            AdresId.Create(Adresbron.AR, "waarde"),
            null);

        locatie1.Equals(locatie2).Should().BeTrue();
    }
}
