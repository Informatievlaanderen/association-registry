namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Hoofdactiviteit;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Known_Code
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
        var hoofdactiviteit = HoofdactiviteitVerenigingsloket.Create(code);
        hoofdactiviteit.Should().BeEquivalentTo(HoofdactiviteitVerenigingsloket.All().Single(h => h.Code == code));
    }
}
