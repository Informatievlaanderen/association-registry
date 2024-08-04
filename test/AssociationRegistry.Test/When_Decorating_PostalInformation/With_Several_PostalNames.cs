namespace AssociationRegistry.Test.When_Decorating_PostalInformation;

using Events;
using FluentAssertions;
using Grar.Models;
using Xunit;

public class With_Several_PostalNames
{
    [Fact]
    public void And_PostName_Exists_Then_Takes_The_PostalName()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1740", "NothingHam", "België"),
        };

        var result = sut.DecorateWithPostalInformation("Hekelgem", new PostalInformationResponse("1741", "Affligem", new[] { "AFFLIGEM", "Essene", "Hekelgem", "Teralfene" }));

        result.Adres.Gemeente.Should().Be("Hekelgem (Affligem)");
    }

    [Fact]
    public void And_PostName_Does_Not_Exists_Then_Takes_The_MunicipalityName()
    {
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1740", "NothingHam", "België"),
        };

        var result = sut.DecorateWithPostalInformation("Nothingham", new PostalInformationResponse("1741", "Affligem", new[] { "AFFLIGEM", "Essene", "Hekelgem", "Teralfene" }));

        result.Adres.Gemeente.Should().Be("Affligem");
    }
}
