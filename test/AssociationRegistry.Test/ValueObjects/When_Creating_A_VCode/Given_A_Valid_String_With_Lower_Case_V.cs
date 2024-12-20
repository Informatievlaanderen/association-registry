namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VCode;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Valid_String_With_Lower_Case_V
{
    [Theory]
    [InlineData("v0001001", "V0001001")]
    [InlineData("v0001002", "V0001002")]
    [InlineData("v0458565", "V0458565")]
    [InlineData("v9999999", "V9999999")]
    public void Then_it_returns_a_new_vcode_formatted_as_V0000000(string strCode, string expectedVCode)
    {
        var vCode = VCode.Create(strCode);

        vCode.Value.Should().Be(expectedVCode);
    }
}
