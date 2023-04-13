namespace AssociationRegistry.Test.When_Creating_A_VCode;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
