namespace AssociationRegistry.Test.When_Creating_An_Insz;

using FluentAssertions;
using Framework;
using INSZ;
using Xunit;

public class Given_A_Valid_String
{
    // Use non existing insz !!
    [Theory]
    [InlineData(InszTestSet.Insz1, InszTestSet.Insz1)]
    [InlineData(InszTestSet.Insz2_WithCharacters, InszTestSet.Insz2)]
    public void Then_it_Returns_A_Insz(string inszString, string expectedInsz)
    {
        string insz = Insz.Create(inszString);
        insz.Should().Be(expectedInsz);
    }
}
