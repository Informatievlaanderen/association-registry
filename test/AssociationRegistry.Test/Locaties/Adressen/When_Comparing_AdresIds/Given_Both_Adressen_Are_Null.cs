namespace AssociationRegistry.Test.Locaties.Adressen.When_Comparing_AdresIds;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using Xunit;

public class Given_Both_AdresIds_Are_Null
{
    [Fact]
    public void Then_Return_False()
    {
        var sut = new AdresIdComparer();

        var result = sut.HasDuplicates(null, null);

        Assert.False(result);
    }
}
