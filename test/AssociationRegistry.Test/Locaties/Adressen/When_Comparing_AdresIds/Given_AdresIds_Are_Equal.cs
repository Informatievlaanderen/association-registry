namespace AssociationRegistry.Test.Locaties.Adressen.When_Comparing_AdresIds;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;

public class Given_AdresIds_Are_Equal
{
    [Fact]
    public void Then_Return_True()
    {
        var fixture = new Fixture().CustomizeDomain();
        var sut = new AdresIdComparer();

        var adresId = fixture.Create<AdresId>();

        var result = sut.HasDuplicates(adresId, adresId);

        Assert.True(result);
    }
}
