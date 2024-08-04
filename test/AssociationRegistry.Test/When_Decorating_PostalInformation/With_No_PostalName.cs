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
        var sut = new AdresMatchUitAdressenregister()
        {
            Adres = new Registratiedata.Adres("Prieelstraat", "12", "bus 101", "1740", "NothingHam", "België"),
        };

        var result = sut.DecorateWithPostalInformation("NothingHam", new PostalInformationResponse("1741", "Ternat", Array.Empty<string>()));

        result.Adres.Gemeente.Should().Be("Ternat");
    }
}
