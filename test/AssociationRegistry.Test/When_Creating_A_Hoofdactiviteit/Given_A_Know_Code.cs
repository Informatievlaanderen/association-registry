namespace AssociationRegistry.Test.When_Creating_A_Hoofdactiviteit;

using Activiteiten;
using FluentAssertions;
using Xunit;

public class Given_A_Know_Code
{
    [Theory]
    [InlineData("BIAG")]
    [InlineData("BWWC")]
    [InlineData("DINT")]
    [InlineData("GEWE")]
    [InlineData("HOSP")]
    [InlineData("INOS")]
    [InlineData("JGDW")]
    [InlineData("KLDU")]
    [InlineData("KECU")]
    [InlineData("LAVI")]
    [InlineData("LEBE")]
    [InlineData("MADP")]
    [InlineData("MEDG")]
    [InlineData("MROW")]
    [InlineData("NMDW")]
    [InlineData("VORM")]
    [InlineData("SOVO")]
    [InlineData("SPRT")]
    [InlineData("ONWE")]
    [InlineData("TOER")]
    [InlineData("WESE")]
    public void Then_it_returns_a_hoofdactiviteit(string code)
    {
        var hoofdactiviteit = Hoofdactiviteit.Create(code);
        hoofdactiviteit.Should().BeEquivalentTo(Hoofdactiviteit.All().Single(h => h.Code == code));
    }
}
