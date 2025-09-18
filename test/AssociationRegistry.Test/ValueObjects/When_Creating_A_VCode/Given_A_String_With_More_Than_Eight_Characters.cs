namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VCode;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_With_More_Than_Eight_Characters
{
    [Theory]
    [InlineData("V12345678")]
    [InlineData("V12345678901234567890")]
    public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
    {
        var ctor = () => VCode.Create(strCode);
        ctor.Should().Throw<VCodeFormaatIsOngeldig>();
    }
}
