namespace AssociationRegistry.Test.When_Creating_A_VCode;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_Not_Starting_With_V
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("A1234567")]
    [InlineData("ABCDEFGH")]
    public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
    {
        var ctor = () => VCode.Create(strCode);
        ctor.Should().Throw<InvalidVCodeFormat>();
    }
}
