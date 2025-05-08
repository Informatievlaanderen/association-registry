namespace AssociationRegistry.Test.ValueObjects.When_Creating_An_AdresId;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_Adresbron_And_Waarde
{
    [Fact]
    public void Then_it_returns_a_new_AdresId()
    {
        var waarde = "https://data.vlaanderen.be/id/adres/0";
        var adresId = AdresId.Create(Adresbron.AR, waarde);

        adresId.Bronwaarde.Should().Be(waarde);
        adresId.Adresbron.Should().Be(Adresbron.AR);
    }
}
