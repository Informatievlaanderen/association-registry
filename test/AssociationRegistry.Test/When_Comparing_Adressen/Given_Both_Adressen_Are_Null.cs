namespace AssociationRegistry.Test.When_Comparing_Adressen;

using Normalizers;
using Vereniging;
using Xunit;

public class Given_Both_Adressen_Are_Null
{
    [Fact]
    public void Then_Return_False()
    {
        var sut = new AdresComparer(new StringStringNormalizer());

        var result = sut.HasDuplicates(null, null);

        Assert.False(result);
    }
}
