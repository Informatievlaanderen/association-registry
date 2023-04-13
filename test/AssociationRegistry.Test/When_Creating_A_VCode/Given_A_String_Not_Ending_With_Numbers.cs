namespace AssociationRegistry.Test.When_Creating_A_VCode;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_Not_Ending_With_Numbers
{
    [Theory]
    [InlineData("VBCDEFGH")]
    [InlineData("V12345AB")]
    public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
    {
        var ctor = () => VCode.Create(strCode);
        ctor.Should().Throw<InvalidVCodeFormat>();
    }
}
