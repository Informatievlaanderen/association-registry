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

        var result = GemeentenaamDecorator.DecorateWithPostalInformation(sut, origineleGemeentenaam: "Halle",
                                                                         postalInformationResponse: new PostalInformationResponse(
                                                                             Postcode: "1501", Gemeentenaam: "Halle", new[] { "Buizingen" }));

        result.Adres.Gemeente.Should().Be("Buizingen (Halle)");
    }

    [Fact]
    public void Then_Takes_The_PostalName_And_MunicipalityName_NothingHam()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "NothingHam", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateWithPostalInformation(sut, origineleGemeentenaam: "NothingHam",
                                                                         postalInformationResponse: new PostalInformationResponse(
                                                                             Postcode: "1741", Gemeentenaam: "Ternat", new[] { "Wambeek" }));

        result.Adres.Gemeente.Should().Be("Wambeek (Ternat)");
    }

    [Fact]
    public void And_MunicipalityName_Equals_PostalName_Then_Returns_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "NothingHam", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateWithPostalInformation(sut, origineleGemeentenaam: "NothingHam",
                                                                         postalInformationResponse: new PostalInformationResponse(
                                                                             Postcode: "1741", Gemeentenaam: "Ternat", new[] { "TERNAT" }));

        result.Adres.Gemeente.Should().Be("Ternat");
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

        var result = GemeentenaamDecorator.DecorateWithPostalInformation(sut, origineleGemeentenaam,
                                                                         new PostalInformationResponse(
                                                                             Postcode: "1741", Gemeentenaam: "Affligem", new[] { "Hekelgem" }));

        result.Adres.Gemeente.Should().Be("Hekelgem (Affligem)");
    }

    [Fact]
    public void And_Municipality_Already_Incorrectly_Formatted()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Fosselstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "Hekelgem", Land: "België"),
        };

        var result = GemeentenaamDecorator.DecorateWithPostalInformation(sut, origineleGemeentenaam: "Hekelgem Affligem",
                                                                         postalInformationResponse: new PostalInformationResponse(
                                                                             Postcode: "1741", Gemeentenaam: "Affligem", new[] { "Hekelgem" }));

        result.Adres.Gemeente.Should().Be("Hekelgem (Affligem)");
    }
}
