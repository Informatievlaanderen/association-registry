namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using Events;
using FluentAssertions;
using Grar.Models;
using Xunit;

public class With_No_PostalName
{
    [Fact]
    public void Then_Takes_The_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister
        {
            Adres = new Registratiedata.Adres(Straatnaam: "Prieelstraat", Huisnummer: "12", Busnummer: "bus 101", Postcode: "1740",
                                              Gemeente: "NothingHam", Land: "België"),
        };

        var result = sut.DecorateWithPostalInformation(origineleGemeentenaam: "NothingHam",
                                                       new PostalInformationResponse(
                                                           Postcode: "1741", Gemeentenaam: "Ternat", Array.Empty<string>()));

        result.Adres.Gemeente.Should().Be("Ternat");
    }
}
