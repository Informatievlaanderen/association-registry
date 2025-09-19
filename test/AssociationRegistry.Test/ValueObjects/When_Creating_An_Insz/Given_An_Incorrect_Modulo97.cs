namespace AssociationRegistry.Test.ValueObjects.When_Creating_An_Insz;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Incorrect_Modulo97
{
    [Theory]
    [InlineData("01234567890")]
    [InlineData("00000000000")]
    [InlineData("01.23.45-678.90")]
    [InlineData("01.23.45--678.90")]
    [InlineData("01.23..45-678-90")]
    public void Then_it_throws_an_InvalidInszMod97Exception(string insz)
    {
        var factory = () => Insz.Create(insz);
        factory.Should().Throw<InszMod97IsOngeldig>();
    }
}
