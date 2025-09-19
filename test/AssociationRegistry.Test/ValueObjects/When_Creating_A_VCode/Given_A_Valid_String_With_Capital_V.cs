namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VCode;

using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_String_With_Capital_V
{
    [Theory]
    [InlineData("V0001001")]
    [InlineData("V0001002")]
    [InlineData("V0845685")]
    [InlineData("V9999999")]
    public void Then_it_returns_a_new_vcode_formatted_as_V0000000(string strCode)
    {
        var vCode = VCode.Create(strCode);

        vCode.Value.Should().Be(strCode);
    }
}
