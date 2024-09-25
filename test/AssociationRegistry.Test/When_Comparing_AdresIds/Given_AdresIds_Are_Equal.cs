namespace AssociationRegistry.Test.When_Comparing_AdresIds;

using AutoFixture;
using Common.AutoFixture;
using Vereniging;
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
