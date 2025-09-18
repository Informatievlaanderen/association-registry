namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VCode;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_String_With_Less_Than_Eight_Characters
{
    [Theory]
    [InlineData("V123456")]
    [InlineData("V1")]
    public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
    {
        var ctor = () => VCode.Create(strCode);
        ctor.Should().Throw<VCodeFormaatIsOngeldig>();
    }
}
