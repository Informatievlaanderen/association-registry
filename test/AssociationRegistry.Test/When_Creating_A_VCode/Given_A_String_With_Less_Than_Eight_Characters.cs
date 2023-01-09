namespace AssociationRegistry.Test.When_Creating_A_VCode;

using FluentAssertions;
using VCodes;
using VCodes.Exceptions;
using Xunit;

public class Given_A_String_With_Less_Than_Eight_Characters
{
    [Theory]
    [InlineData("V123456")]
    [InlineData("V1")]
    public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
    {
        var ctor = () => VCode.Create(strCode);
        ctor.Should().Throw<InvalidVCodeFormat>();
    }
}
