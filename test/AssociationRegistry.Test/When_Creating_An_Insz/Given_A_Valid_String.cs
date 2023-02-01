namespace AssociationRegistry.Test.When_Creating_An_Insz;

using FluentAssertions;
using INSZ;
using Xunit;

public class Given_A_Valid_String
{
    // Use non existing insz !!
    [Theory]
    [InlineData("01131500149", "01131500149")]
    [InlineData("03.20.98-203.96", "03209820396")]
    public void Then_it_Returns_A_Insz(string inszString, string expectedInsz)
    {
        string insz = Insz.Create(inszString);
        insz.Should().Be(expectedInsz);
    }
}
