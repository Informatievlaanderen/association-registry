namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using Events;
using FluentAssertions;
using Grar.Models;
using Xunit;

public class With_One_PostalName
{
    [Fact]
    public void Then_Takes_The_PostalName_And_MunicipalityName_Halle()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1501",
                                              Gemeente: "Halle", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateGemeentenaam(origineleGemeentenaam: "Halle",
                                                                postalInformationResponse: new PostalInformationResponse(
                                                                    Postcode: "1501", Gemeentenaam: "Halle", new[] { "Buizingen" }), gemeentenaamUitAdresmatch: sut.Adres.Gemeente);

        result.Should().Be("Buizingen (Halle)");
    }

    [Fact]
    public void Then_Takes_The_PostalName_And_MunicipalityName_NothingHam()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "NothingHam", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateGemeentenaam(origineleGemeentenaam: "NothingHam",
                                                                postalInformationResponse: new PostalInformationResponse(
                                                                    Postcode: "1741", Gemeentenaam: "Ternat", new[] { "Wambeek" }), gemeentenaamUitAdresmatch: sut.Adres.Gemeente);

        result.Should().Be("Wambeek (Ternat)");
    }

    [Fact]
    public void And_MunicipalityName_Equals_PostalName_Then_Returns_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "NothingHam", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateGemeentenaam(origineleGemeentenaam: "NothingHam",
                                                                postalInformationResponse: new PostalInformationResponse(
                                                                    Postcode: "1741", Gemeentenaam: "Ternat", new[] { "TERNAT" }), gemeentenaamUitAdresmatch: sut.Adres.Gemeente);

        result.Should().Be("Ternat");
    }

    [Theory]
    [InlineData("Hekelgem (Affligem)")]
    [InlineData("Hekelgem (afg)")]
    [InlineData("Hekelgem")]
    public void And_Municipality_Already_Correctly_Formatted(string origineleGemeentenaam)
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Fosselstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "Hekelgem", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateGemeentenaam(origineleGemeentenaam,
                                                                new PostalInformationResponse(
                                                                    Postcode: "1741", Gemeentenaam: "Affligem", new[] { "Hekelgem" }), sut.Adres.Gemeente);

        result.Should().Be("Hekelgem (Affligem)");
    }

    [Fact]
    public void And_Municipality_Already_Incorrectly_Formatted()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Fosselstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "Hekelgem", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateGemeentenaam(origineleGemeentenaam: "Hekelgem Affligem",
                                                                postalInformationResponse: new PostalInformationResponse(
                                                                    Postcode: "1741", Gemeentenaam: "Affligem", new[] { "Hekelgem" }), gemeentenaamUitAdresmatch: sut.Adres.Gemeente);

        result.Should().Be("Hekelgem (Affligem)");
    }
}
