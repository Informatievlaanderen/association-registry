namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Hoofdactiviteit;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Unknown_Code
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("RANDOM")]
    public void Then_it_throws_an_UnknownHoofdactiviteitCodeException(string code)
    {
        var ctor = () => HoofdactiviteitVerenigingsloket.Create(code);
        ctor.Should().Throw<HoofdactiviteitCodeIsNietGekend>();
    }
}
