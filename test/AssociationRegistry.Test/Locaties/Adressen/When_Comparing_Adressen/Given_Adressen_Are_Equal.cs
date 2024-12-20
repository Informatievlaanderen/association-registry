namespace AssociationRegistry.Test.Locaties.Adressen.When_Comparing_Adressen;

using AssociationRegistry.Normalizers;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;

public class Given_Adressen_Are_Equal
{
    [Fact]
    public void Then_Return_True()
    {
        var fixture = new Fixture().CustomizeDomain();
        var sut = new AdresComparer(new AdresComponentNormalizer());

        var adres = fixture.Create<Adres>();

        var result = sut.HasDuplicates(adres, adres);

        Assert.True(result);
    }
}
