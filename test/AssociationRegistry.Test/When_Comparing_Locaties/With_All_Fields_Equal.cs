namespace AssociationRegistry.Test.When_Comparing_Locaties;

using Admin.Api.Constants;
using FluentAssertions;
using Vereniging;
using Xunit;

public class With_All_Fields_Equal
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
            Adres.Create("straatnaam",
                "huisnummer",
                "busnummer",
                "postCode",
                "gemeente",
                "land"));

        locatie1.Equals(locatie2).Should().BeTrue();
    }
}
