namespace AssociationRegistry.Test.ValueObjects.When_Creating_An_Insz;

using AssociationRegistry.Test.Framework;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_String
{
    // Use non existing insz !!
    [Theory]
    [InlineData(InszTestSet.Insz1, InszTestSet.Insz1)]
    [InlineData(InszTestSet.Insz2_WithCharacters, InszTestSet.Insz2)]
    [InlineData(InszTestSet.Bis1, InszTestSet.Bis1)]
    [InlineData(InszTestSet.Bis2, InszTestSet.Bis2)]
    [InlineData(InszTestSet.Bis3, InszTestSet.Bis3)]
    [InlineData(InszTestSet.Bis4, InszTestSet.Bis4)]
    public void Then_it_Returns_A_Insz(string inszString, string expectedInsz)
    {
        string insz = Insz.Create(inszString);
        insz.Should().Be(expectedInsz);
    }
}
