namespace AssociationRegistry.Test.Grar.NutsLau;

using AssociationRegistry.Grar.NutsLau;
using Xunit;

public class Given_Brusselse_Postcodes
{
    [Theory]
    [InlineData("1000", true)]
    [InlineData("1200", true)]
    [InlineData("1299", true)]

    [InlineData("1400", false)]
    [InlineData("4000", false)]
    [InlineData("7000", false)]
    [InlineData("10000", false)]
    [InlineData("abcd", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsFlemishPostcode_ShouldWorkCorrectly(string postcode, bool expected)
    {
        var result = Postcode.IsBrusselPostcode(postcode);

        Assert.Equal(expected, result);
    }
}

