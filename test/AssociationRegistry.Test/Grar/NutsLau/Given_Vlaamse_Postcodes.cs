namespace AssociationRegistry.Test.Grar.NutsLau;

using AssociationRegistry.Grar.NutsLau;
using Xunit;

public class Given_Vlaamse_Postcodes
{
    [Theory]
    // Valid Flemish postcodes
    [InlineData("1500", true)]
    [InlineData("1999", true)]
    [InlineData("2000", true)]
    [InlineData("3999", true)]
    [InlineData("8000", true)]
    [InlineData("9999", true)]

    // Invalid (Walloon, Brussels, or malformed)
    [InlineData("1000", false)]
    [InlineData("1400", false)]
    [InlineData("4000", false)]
    [InlineData("7000", false)]
    [InlineData("10000", false)]
    [InlineData("abcd", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsFlemishPostcode_ShouldWorkCorrectly(string postcode, bool expected)
    {
        var result = Postcode.IsVlaamsePostcode(postcode);

        Assert.Equal(expected, result);
    }
}

