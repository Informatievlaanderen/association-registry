namespace AssociationRegistry.Test.Locaties.Adressen.When_Comparing_Adressen;

using AssociationRegistry.Normalizers;
using AssociationRegistry.Vereniging;
using Xunit;

public class Given_Both_Adressen_Are_Null
{
    [Fact]
    public void Then_Return_False()
    {
        var sut = new AdresComparer(new AdresComponentNormalizer());

        var result = sut.HasDuplicates(null, null);

        Assert.False(result);
    }
}
