namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VCode;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
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
        ctor.Should().Throw<VCodeFormaatIsOngeldig>();
    }
}
