namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using Events;
using Grar.Models;
using FluentAssertions;
using Xunit;

public class With_One_PostalName
{
    [Fact]
    public void Then_Takes_The_PostalName_And_MunicipalityName_Halle()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1501", "Halle", "Belgie"),
        };

        var result = sut.DecorateWithPostalInformation("Halle", new PostalInformationResponse("1501", "Halle", new[] { "Buizingen" }));

        result.Adres.Gemeente.Should().Be("Buizingen (Halle)");
    }

    [Fact]
    public void Then_Takes_The_PostalName_And_MunicipalityName_NothingHam()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1740", "NothingHam", "Belgie"),
        };

        var result = sut.DecorateWithPostalInformation("NothingHam", new PostalInformationResponse("1741", "Ternat", new[] { "Wambeek" }));

        result.Adres.Gemeente.Should().Be("Wambeek (Ternat)");
    }

    [Fact]
    public void And_MunicipalityName_Equals_PostalName_Then_Returns_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1740", "NothingHam", "Belgie"),
        };

        var result = sut.DecorateWithPostalInformation("NothingHam", new PostalInformationResponse("1741", "Ternat", new[] { "TERNAT" }));

        result.Adres.Gemeente.Should().Be("Ternat");
    }

    [Theory]
    [InlineData("Hekelgem (Affligem)")]
    [InlineData("Hekelgem (afg)")]
    [InlineData("Hekelgem")]
    public void And_Municipality_Already_Correctly_Formatted(string origineleGemeentenaam)
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Fosselstraat", "12", "bus 101", "1740", "Hekelgem", "Belgie"),
        };

        var result = sut.DecorateWithPostalInformation(origineleGemeentenaam, new PostalInformationResponse("1741", "Affligem", new[] { "Hekelgem" }));

        result.Adres.Gemeente.Should().Be("Hekelgem (Affligem)");
    }

    [Fact]
    public void And_Municipality_Already_Incorrectly_Formatted()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Fosselstraat", "12", "bus 101", "1740", "Hekelgem", "Belgie"),
        };

        var result = sut.DecorateWithPostalInformation("Hekelgem Affligem", new PostalInformationResponse("1741", "Affligem", new[] { "Hekelgem" }));

        result.Adres.Gemeente.Should().Be("Hekelgem (Affligem)");
    }
}
