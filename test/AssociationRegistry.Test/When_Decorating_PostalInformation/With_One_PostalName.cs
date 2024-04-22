namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using Events;
using Grar.Models;
using FluentAssertions;
using Xunit;

public class With_One_PostalName
{
    [Fact]
    public void Then_Takes_The_PostalName_And_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1740", "NothingHam", "Belgie")
        };

        var result = sut.DecorateWithPostalInformation("NothingHam", new PostalInformationResponse("1741", "Ternat", new[] { "Wambeek" }));

        result.Adres.Gemeente.Should().Be("Wambeek (Ternat)");
    }

    [Fact]
    public void And_MunicipalityName_Equals_PostalName_Then_Returns_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1740", "NothingHam", "Belgie")
        };

        var result = sut.DecorateWithPostalInformation("NothingHam", new PostalInformationResponse("1741", "Ternat", new[] { "TERNAT" }));

        result.Adres.Gemeente.Should().Be("Ternat");
    }
}
