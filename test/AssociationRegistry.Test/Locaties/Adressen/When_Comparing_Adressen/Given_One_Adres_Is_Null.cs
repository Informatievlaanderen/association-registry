namespace AssociationRegistry.Test.Locaties.Adressen.When_Comparing_Adressen;

using AssociationRegistry.Normalizers;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging.Adressen;
using Xunit;

public class Given_One_Adres_Is_Null
{
    [Fact]
    public void Then_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var sut = new AdresComparer(new AdresComponentNormalizer());

        var result = sut.HasDuplicates(fixture.Create<Adres>(), null);

        Assert.False(result);
    }

    [Fact]
    public void With_Other_Adres_Null_Then_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var sut = new AdresComparer(new AdresComponentNormalizer());

        var result = sut.HasDuplicates(null, fixture.Create<Adres>());

        Assert.False(result);
    }
}
