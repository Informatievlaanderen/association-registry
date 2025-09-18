namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VCode;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_Not_Starting_With_V
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("A1234567")]
    [InlineData("ABCDEFGH")]
    public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
    {
        var ctor = () => VCode.Create(strCode);
        ctor.Should().Throw<VCodeFormaatIsOngeldig>();
    }
}
